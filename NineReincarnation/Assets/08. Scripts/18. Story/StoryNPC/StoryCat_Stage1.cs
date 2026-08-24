using Cysharp.Threading.Tasks;
using DialogueSpace;
using UnityEngine;

public class StoryCat_Stage1 : StoryCat
{
    [Header("----Thread----")]
    [SerializeField] private StoryThread _storyThread;
    [SerializeField] private Transform _threadCatPoint;
    [SerializeField] private Transform _threadAnnaPoint;
    [SerializeField] private GameObject _threadBall;

    protected override void Awake()
    {
        base.Awake();

        _dialogueManager.DialogueEndAddListener(DisconnectThread);
    }

    public override async UniTask ExecuteEvent(int index)
    {
        if (index == 0)
        {
            await MoveToTarget(false);
        }
        else if (index == 1)
        {
            _storyThread.Connect(_threadAnnaPoint, _threadCatPoint);
            await RunAndDisappear(true);
        }
        else if (index == 2)
        {
            _dialogueManager.DialogueEndAddListener(DisableSelf);
            await RunAndPlayAnimation("isFallOff");
        }
    }

    public override void FinishEvent(int index)
    {
        if (index == 0)
        {
            FinishMoveToTarget();
        }
        else if (index == 1)
        {
            FinishRunAndDisapper(true);
        }
        else if (index == 2)
        {
            _dialogueManager.DialogueEndAddListener(DisableSelf);
            FinishRunAndShowBall();
        }
    }

    protected override async UniTask RunAndPlayAnimation(string _animationName)
    {
        _speed = _runSpeed;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true, false);
        _threadBall.SetActive(true);
        _isAnimationEnd = false;
        NpcAnimator.SetTrigger(_animationName);
        await UniTask.WaitUntil(() => _isAnimationEnd == true);
    }

    private void FinishRunAndShowBall()
    {
        FinishMoveToTarget(false);
        _threadBall.SetActive(true);
    }

    private void DisconnectThread()
    {
        _storyThread.Disconnect();
    }

}
