using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class StoryTimeTrigger : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private float _stayWaitTime = 3f;
    [SerializeField] private bool _synchronizePlayerPos = false;

    private bool isWaitStarted = false;
    private bool isTriggered = false;
    private CancellationTokenSource _cts;

    private void Start()
    {
        _cts = new CancellationTokenSource();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;
        if (isWaitStarted) return;
        if (!collision.CompareTag(_playerTag)) return;

        isWaitStarted = true;
        WaitAndStartDialogue(_cts.Token).Forget();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTriggered) return;
        if (!collision.CompareTag(_playerTag)) return;
        if (isWaitStarted == false) return;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(
            this.GetCancellationTokenOnDestroy()
        );

        isWaitStarted = false;
    }

    public async UniTaskVoid WaitAndStartDialogue(CancellationToken token)
    {
        try
        {
            //Debug.Log("트리거 타이머 시작");

            await UniTask.WaitForSeconds(_stayWaitTime, cancellationToken: token);

            isTriggered = true;

            //Debug.Log("트리거 발동 완료");

            if(_synchronizePlayerPos)
            {
                _dialogueManager.SynchronizePlayerPos();
            }

            _dialogueManager.DialogueExctute(_id).Forget();
        }
        catch (OperationCanceledException)
        {
            //Debug.Log("트리거 타이머 종료됨");
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
