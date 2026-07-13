using AnyPortrait;
using Cysharp.Threading.Tasks;
using ExcelData;
using Player.Controller;
using System.Collections.Generic;
using System.Threading;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace DialogueSpace
{
    public class DialogueManager : MonoBehaviour
    {
        [Inject] private DialogueUI _dialogueUI;
        [Inject] private DialogueDB _dialogueDB;
        [Inject] private StoryAnimationManager _storyAnimationManager;
        [Inject] private StoryEventManager _storyEventManager;
        [Inject] private BubbleManager _bubbleManager;
        [Inject] private SelectManager _selectManager;
        [Inject] private PlayerController _anna;
        [SerializeField] private GameObject _npcAnna;
        [SerializeField] private int _id;
        [SerializeField] private EventCamera _camera;
        [SerializeField] private VirtualCameraManager _virtualCameraManager;
        [SerializeField] private bool _startScene = false;

        List<UniTask> tasks = new List<UniTask>(5);

        List<UniTask>[] subTasks; // 병렬 처리용 테스크
        private const int maxSubTaskCount = 2;

        List<UniTask> totalTasks; // 전체 테스크
        
        private CancellationTokenSource _cts;
        private int _nextId;

        private UnityEvent OnDialogueEnd = new UnityEvent();

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            _cts.Token.RegisterWithoutCaptureExecutionContext(ResetState);

            subTasks = new List<UniTask>[maxSubTaskCount];
            for (int i = 0; i < maxSubTaskCount; ++i)
            {
                subTasks[i] = new List<UniTask>(5);
            }

            totalTasks = new List<UniTask>(maxSubTaskCount + 1);
        }

        private void OnEnable()
        {
            if (_startScene)
            {
                if (_anna.gameObject.activeSelf)
                {
                    _anna.gameObject.SetActive(false);
                    _npcAnna.SetActive(true);
                }
            }
        }

        private void Start()
        {
            DialogueStart();
        }
        
        private void DialogueStart()
        {
            Debug.Log("다이얼로그 시작");
            DialogueExctute(_id).Forget();
        }

        private void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        public async UniTaskVoid DialogueExctute(int id)
        {
            _id = id;

            bool isNext = true;

            if (_anna.gameObject.activeSelf)
            {
                _anna.gameObject.SetActive(false);
                _npcAnna.SetActive(true);
            }

            while (isNext)
            {
                isNext = await NextDialogue();
            }
        }

        private async UniTask<bool> NextDialogue()
        {
            try
            {
                DialogueClass dialogue = _dialogueDB.GetData<DialogueClass>(_id);

                _nextId = dialogue.NextID;

                //End면 종료
                if (dialogue.EventType == ExcelData.EventType.End)
                {
                    await _camera.ZoomDefault();

                    _anna.transform.position = _npcAnna.transform.position;
                    _npcAnna.SetActive(false);
                    _anna.gameObject.SetActive(true);
                    _virtualCameraManager.SetPlayer();

                    OnDialogueEnd?.Invoke();
                    OnDialogueEnd.RemoveAllListeners();

                    return false;
                }

                // 지금 차례 테스크 추가
                TaskAdder(tasks, dialogue);
                totalTasks.Add(StartMainTask(tasks, dialogue.Duration));

                // 선택지, 스크립트 이벤트가 있다면 병렬 처리 안함(선택지는 id 꼬일 수 있음, 스크립트는 동시 출력 불가)
                if (dialogue.IsThisEvent(ExcelData.EventType.Select) == false &&
                    dialogue.IsThisEvent(ExcelData.EventType.Script) == false &&
                    dialogue.IsThisEvent(ExcelData.EventType.Event) == false) 
                {
                    // 병렬 처리 테스크 추가
                    int curSubTaskIndex = 0; int subID = _nextId;
                    DialogueClass nextDialogue = _dialogueDB.GetData<DialogueClass>(subID);
                    while (curSubTaskIndex < maxSubTaskCount && nextDialogue.IsThisEvent(ExcelData.EventType.Parallel))
                    {
                        subID = _nextId; _nextId = nextDialogue.NextID;

                        ParallelClass data = _dialogueDB.GetData<ParallelClass>(subID);
                        totalTasks.Add(StartSubTask(subTasks[curSubTaskIndex], nextDialogue, data.TimeOffset));

                        // 다음 다이얼로그 처리
                        nextDialogue = _dialogueDB.GetData<DialogueClass>(_nextId);
                        ++curSubTaskIndex;
                    }
                }

                // 테스크 총괄 실행
                await UniTask.WhenAll(totalTasks).AttachExternalCancellation(_cts.Token);

                if (_bubbleManager.HasSkipEvent || _camera.HasSkipEvent || _dialogueUI.HasSkipEvent)
                {
                    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: _cts.Token);

                    await FinishEvent();
                }

                _id = _nextId;
            }
            finally
            {
                tasks.Clear();
                for(int i = 0; i < maxSubTaskCount; ++i)
                {
                    subTasks[i].Clear();
                }
                totalTasks.Clear();
            }

            return true;
        }
        private async UniTask SelectWrapper(SelectClass data, SelectDataStruct[] selectDataes)
        {
            _nextId = await _selectManager.ExcuteSelect(data, selectDataes);
        }

        private async UniTask FinishEvent()
        {
            if (_camera.HasSkipEvent)
            {
                await _camera.CancelShake();
            }

            if (_bubbleManager.HasSkipEvent)
            {
                _bubbleManager.CloseBubble();
            }

            if (_dialogueUI.HasSkipEvent)
            {
                _dialogueUI.CloseUI();
            }

            await UniTask.NextFrame();
        }

        private async UniTask StartMainTask(List<UniTask> taskList, float duration)
        {
            await UniTask.WhenAll(taskList).AttachExternalCancellation(_cts.Token);
            await UniTask.WaitForSeconds(duration, cancellationToken: _cts.Token);
        }

        private async UniTask StartSubTask(List<UniTask> taskList, DialogueClass dialogue, float timeOffset)
        {
            await UniTask.WaitForSeconds(timeOffset, cancellationToken: _cts.Token);

            TaskAdder(taskList, dialogue, true);

            await UniTask.WhenAll(taskList).AttachExternalCancellation(_cts.Token);
            await UniTask.WaitForSeconds(dialogue.Duration, cancellationToken: _cts.Token);
        }

        private void ResetState()
        {
            if (_camera != null && _camera.HasSkipEvent)
                _camera.StopShakeImmediate();

            if (_bubbleManager != null && _bubbleManager.HasSkipEvent)
                _bubbleManager.CloseBubble();

            if (_dialogueUI != null && _dialogueUI.HasSkipEvent)
                _dialogueUI.CloseUI();
        }

        private void TaskAdder(List<UniTask> taskList, DialogueClass dialogue, bool isSubTask = false)
        {
            if (isSubTask == false && dialogue.IsThisEvent(ExcelData.EventType.Script))
            {
                taskList.Add(_dialogueUI.UpdateUI(_dialogueDB.GetData<ScriptClass>(dialogue.ID), _cts.Token));
            }
            if (isSubTask == false && dialogue.IsThisEvent(ExcelData.EventType.Event))
            {
                taskList.Add(_storyEventManager.ExcuteEvent(_cts, dialogue.ID));
            }
            if (isSubTask == false && dialogue.IsThisEvent(ExcelData.EventType.Select))
            {
                SelectClass data = _dialogueDB.GetData<SelectClass>(dialogue.ID);

                int size = data.ChoiceCount;

                SelectDataStruct[] selectDataStructs = new SelectDataStruct[size];

                for (int i = 1; i <= size; ++i)
                {
                    int sid = dialogue.ID * 10 + i;
                    string script = _dialogueDB.GetData<ScriptClass>(sid).Script;
                    int nextId = _dialogueDB.GetData<DialogueClass>(sid).NextID;
                    selectDataStructs[i - 1].SetSelectDataStruct(sid, nextId, script);
                }

                taskList.Add(SelectWrapper(data, selectDataStructs));
            }

            // 병렬 처리가 가능한 테스크
            if (dialogue.IsThisEvent(ExcelData.EventType.Camera))
            {
                taskList.Add(_camera.ExcuteEvent(_dialogueDB.GetData<CameraClass>(dialogue.ID)));
            }
            if (dialogue.IsThisEvent(ExcelData.EventType.Animation))
            {
                taskList.Add(_storyAnimationManager.ExcuteAnimation(_dialogueDB.GetData<AnimationClass>(dialogue.ID)));
            }
            if (dialogue.IsThisEvent(ExcelData.EventType.Bubble))
            {
                taskList.Add(_bubbleManager.ExcuteBubble(_dialogueDB.GetData<BubbleClass>(dialogue.ID)));
            }
        }

        public void SynchronizePlayerPos()
        {
            _npcAnna.transform.position = _anna.transform.position;
        }

        public void DialogueEndAddListener(UnityAction action)
        {
            OnDialogueEnd.RemoveListener(action);
            OnDialogueEnd.AddListener(action);
        }
    }
}