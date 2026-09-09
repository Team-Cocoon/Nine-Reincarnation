using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.Controller;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class StoryKeyEventTrigger : EventTrigger
{
    [Inject] private InputManager _inputManager;
    [Inject] private PlayerController _mainCharacter;

    [SerializeField] private bool _isRepeatable = false;

    private bool _isInTheZone = false;
    private bool _isShowingDialogue = false;
    private CancellationTokenSource _cts;

    [Header("----- NPC -----")]
    [SerializeField] private StoryNPC _npcMainCharacter;
    [SerializeField] private Transform _mainCharacterStartPos;
    [SerializeField] private PlayerDirection _mainStartFlip;
    [SerializeField] private List<StoryNPC> _npcList = new List<StoryNPC>();
    [SerializeField] private List<GameObject> _realList = new List<GameObject>();
    [SerializeField] private List<Transform> _extraStartPos = new List<Transform>();
    [SerializeField] private List<PlayerDirection> _extraStartFlip = new List<PlayerDirection>();
    private int _extraCount;

    List<UniTask> _npcTasks;

    [Header("----- UI -----")]
    [SerializeField] private GameObject _keyUI;
    [SerializeField] private TMP_Text _keyInfoTxt;
    [SerializeField] private string _keyInformText;

    private void Awake()
    {
        if(_keyUI == null || _keyInfoTxt == null)
        {
            Debug.LogError("No Key UI or Key Info Text");
            gameObject.SetActive(false);
            return;
        }

        _keyUI.SetActive(false);
        _keyInfoTxt.text = _keyInformText;

        _isShowingDialogue = _isInTheZone = false;

        _cts = new CancellationTokenSource();

        _extraCount = Mathf.Min(_npcList.Count, _realList.Count, 
            _extraStartPos.Count, _extraStartFlip.Count);
        _npcTasks = new List<UniTask>(_extraCount + 1);
    }

    // 임시
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.F))
    //        PrepareDialogue();
    //}

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigger) return; 
        if (!collision.CompareTag(_playerTag)) return;

        _isInTheZone = true;
        _keyUI.SetActive(true);
        _inputManager.AddListenerToInput(InputManager.InputMap.Player, 
            InputManager.InputActions.StartDialogue, PrepareDialogue);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTrigger) return;
        if (!collision.CompareTag(_playerTag)) return;

        _isInTheZone = false;
        _keyUI.SetActive(false);
        _inputManager.RemoveListenerInInput(InputManager.InputMap.Player,
            InputManager.InputActions.StartDialogue, PrepareDialogue);
    }

    public void PrepareDialogue(InputAction.CallbackContext context)
    {
        Debug.Log("F 키 눌림");

        if (_isShowingDialogue || _isInTheZone == false || isTrigger == true)
            return;

        if (_isRepeatable == false) 
            isTrigger = true;

        _isShowingDialogue = true;
        _keyUI.SetActive(false);
        WaitAndStartDialogue(_cts.Token).Forget();
    }
    
    public async UniTaskVoid WaitAndStartDialogue(CancellationToken token)
    {
        // 특정 위치로 이동
        SetOriginPosAndAddToTask(_mainCharacter.gameObject, _npcMainCharacter, _mainCharacterStartPos);

        for(int i = 0; i < _extraCount; ++i)
        {
            SetOriginPosAndAddToTask(_realList[i], _npcList[i], _extraStartPos[i]);
        }

        await UniTask.WhenAll(_npcTasks).AttachExternalCancellation(_cts.Token);

        _npcMainCharacter.Flip(_mainStartFlip);
        for (int i = 0; i < _extraCount; ++i)
        {
            _npcList[i].Flip(_extraStartFlip[i]);
        }

        _dialogueManager.DialogueEndAddListener(OnDialogueEnd);
        StartDialogue();
    }

    private void OnDialogueEnd()
    {
        _isShowingDialogue = false;
    }

    private void SetOriginPosAndAddToTask(GameObject real, StoryNPC npc, Transform targetPos)
    {
        npc.transform.position = real.transform.position;
        npc.gameObject.SetActive(true);
        real.gameObject.SetActive(false);

        _npcTasks.Add(npc.MoveToTarget(targetPos));
    }

    public void ForceQuit()
    {
        _cts.Cancel();
        _npcTasks.Clear();
        _dialogueManager.StopDialogue();
    }
}
