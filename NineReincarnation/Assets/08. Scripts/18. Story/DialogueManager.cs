using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExcelData;
using Player.Controller;
using UnityEngine;
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
        private CancellationTokenSource _cts;
        private int _nextId;
        private void Awake()
        {
            _cts = new CancellationTokenSource();
            _cts.Token.RegisterWithoutCaptureExecutionContext(ResetState);
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
                SceneEventHandler.SceneStarted += DialogueStart;
            }
        }

        private void OnDisable()
        {
            if (_startScene)
            {
                SceneEventHandler.SceneStarted -= DialogueStart;
            }
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

                    return false;
                }

                if ((dialogue.EventType & ExcelData.EventType.Script) == ExcelData.EventType.Script)
                {
                    tasks.Add(_dialogueUI.UpdateUI(_dialogueDB.GetData<ScriptClass>(_id), _cts.Token));
                }
                if ((dialogue.EventType & ExcelData.EventType.Event) == ExcelData.EventType.Event)
                {
                    tasks.Add(_storyEventManager.ExcuteEvent(_cts, _id));
                }
                if ((dialogue.EventType & ExcelData.EventType.Camera) == ExcelData.EventType.Camera)
                {
                    tasks.Add(_camera.ExcuteEvent(_dialogueDB.GetData<CameraClass>(_id)));
                }
                if ((dialogue.EventType & ExcelData.EventType.Animation) == ExcelData.EventType.Animation)
                {
                    tasks.Add(_storyAnimationManager.ExcuteAnimation(_dialogueDB.GetData<AnimationClass>(_id)));
                }
                if ((dialogue.EventType & ExcelData.EventType.Bubble) == ExcelData.EventType.Bubble)
                {
                    tasks.Add(_bubbleManager.ExcuteBubble(_dialogueDB.GetData<BubbleClass>(_id)));
                }
                if ((dialogue.EventType & ExcelData.EventType.Select) == ExcelData.EventType.Select)
                {
                    SelectClass data = _dialogueDB.GetData<SelectClass>(_id);

                    int size = data.ChoiceCount;

                    SelectDataStruct[] selectDataStructs = new SelectDataStruct[size];

                    for (int i = 1; i <= size; ++i)
                    {
                        int id = _id * 10 + i;
                        string script = _dialogueDB.GetData<ScriptClass>(id).Script;
                        int nextId = _dialogueDB.GetData<DialogueClass>(id).NextID;
                        selectDataStructs[i - 1].SetSelectDataStruct(id, nextId, script);
                    }

                    tasks.Add(SelectWrapper(data, selectDataStructs));
                }

                await UniTask.WhenAll(tasks).AttachExternalCancellation(_cts.Token);
                await UniTask.WaitForSeconds(dialogue.Duration, cancellationToken: _cts.Token);

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

        private void ResetState()
        {
            if (_camera != null && _camera.HasSkipEvent)
                _camera.StopShakeImmediate();

            if (_bubbleManager != null && _bubbleManager.HasSkipEvent)
                _bubbleManager.CloseBubble();

            if (_dialogueUI != null && _dialogueUI.HasSkipEvent)
                _dialogueUI.CloseUI();
        }
    }
}