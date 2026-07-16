using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public class StoryCat : StoryNPC, IEventInterface
{
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;

    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private float _speed;
    [SerializeField] private float _runSpeed = 6f;

    [Header("----Thread----")]
    [SerializeField] private StoryThread _storyThread;
    [SerializeField] private Transform _threadCatPoint;
    [SerializeField] private Transform _threadAnnaPoint;
    [SerializeField] private GameObject _threadBall;

    private Queue<Transform> _wayPointQueue;

    private bool _isAnimationEnd = false;

    private void Awake()
    {
        _wayPointQueue = new Queue<Transform>(_wayPoints);
        _dialogueManager.DialogueEndAddListener(DisconnectThread);
    }

    private async UniTask MoveToTarget(bool isRunning, bool isPlayIdleAfter = true)
    {
        if (_wayPoints.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();

        float dist = Vector2.Distance(_rb2d.position, targetTransform.position);

        float sign = Mathf.Sign(targetTransform.position.x - transform.position.x);

        if (sign >= float.Epsilon) Flip(PlayerDirection.Right);
        else Flip(PlayerDirection.Left);

        if (isRunning == false)
        {
            NpcAnimator.SetTrigger("isMove");
        }
        else
        {
            NpcAnimator.SetTrigger("isRun");
        }

        Move(sign);
        AudioManager.Instance?.PlayLoopingSfx(AudioManager.LoopSfx.CatWalk);
        while (dist > 0.1f)
        {
            dist = Mathf.Abs(targetTransform.position.x - _rb2d.position.x);

            await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate);
        }
        
        if(isPlayIdleAfter)
            NpcAnimator.SetTrigger("isIdle");
        AudioManager.Instance?.StopLoopingSfx(AudioManager.LoopSfx.CatWalk);

        _rb2d.linearVelocityX = 0.0f;
    }

    private void Move(float direction)
    {
        _rb2d.linearVelocityX = direction * _speed;
    }

    public void PlayCatJumpSound()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatJump);
    }

    public void PlayCatStrokedSound()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatStroked);
    }

    public void PlayCatPositiveSound()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatPositive);
    }

    public void SynchronizeAnimation()
    {
        if (transform.localPosition == Vector3.zero)
            return;

        _rb2d.MovePosition(_rb2d.position + (Vector2)transform.localPosition);
        transform.localPosition = Vector3.zero;
    }

    public async UniTask ExecuteEvent(int index)
    {
        if (index == 0)
        {
            await MoveToTarget(false);
        }
        else if (index == 1)
        {
            await RunAndDisappear(true);
        }
        else if (index == 2)
        {
            _dialogueManager.DialogueEndAddListener(DisableSelf);
            await RunAndPlayAnimation("isFallOff");
        }
    }

    private async UniTask RunAndDisappear(bool teleportToNextWayPoint = false)
    {
        _storyThread.Connect(_threadAnnaPoint, _threadCatPoint);

        _speed = _runSpeed;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true);
        if(teleportToNextWayPoint)
        {
            Transform tp = _wayPointQueue.Dequeue();
            Debug.Log(tp.name);
            _rb2d.position = tp.position;
            NpcAnimator.SetTrigger("isIdle");
        }
    }

    private async UniTask RunAndPlayAnimation(string _animationName)
    {
        _speed = _runSpeed;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true, false);
        _threadBall.SetActive(true);
        _isAnimationEnd = false;
        NpcAnimator.SetTrigger(_animationName);
        await UniTask.WaitUntil(() => _isAnimationEnd == true);
    }

    private void SetAnimationEnd()
    {
        _isAnimationEnd = true;
    }

    private void DisconnectThread()
    {
        _storyThread.Disconnect();
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
