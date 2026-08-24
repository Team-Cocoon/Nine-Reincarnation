using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public abstract class StoryCat : StoryNPC, IEventInterface
{
    [SerializeField] protected DialogueSpace.DialogueManager _dialogueManager;

    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] protected Rigidbody2D _rb2d;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _runSpeed = 6f;

    protected Queue<Transform> _wayPointQueue;

    protected bool _isAnimationEnd = false;

    protected virtual void Awake()
    {
        _wayPointQueue = new Queue<Transform>(_wayPoints);
    }

    protected virtual async UniTask MoveToTarget(bool isRunning, bool isPlayIdleAfter = true)
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

    protected virtual void FinishMoveToTarget(bool isPlayIdleAfter = true)
    {
        if (_wayPoints.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();

        float sign = Mathf.Sign(targetTransform.position.x - transform.position.x);

        if (sign >= float.Epsilon) Flip(PlayerDirection.Right);
        else Flip(PlayerDirection.Left);

        if (isPlayIdleAfter)
            NpcAnimator.SetTrigger("isIdle");

        _rb2d.linearVelocityX = 0.0f;
        _rb2d.position = targetTransform.position;
    }
    protected void Move(float direction)
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

    protected virtual async UniTask RunAndPlayAnimation(string _animationName)
    {
        _speed = _runSpeed;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true, false);
        _isAnimationEnd = false;
        NpcAnimator.SetTrigger(_animationName);
        await UniTask.WaitUntil(() => _isAnimationEnd == true);
    }

    protected virtual async UniTask RunAndDisappear(bool teleportToNextWayPoint = false)
    {
        _speed = _runSpeed;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true);
        if (teleportToNextWayPoint)
        {
            Transform tp = _wayPointQueue.Dequeue();
            Debug.Log(tp.name);
            _rb2d.position = tp.position;
            NpcAnimator.SetTrigger("isIdle");
        }
    }

    protected virtual void FinishRunAndDisapper(bool teleportToNextWayPoint = false)
    {
        FinishMoveToTarget();
        if (teleportToNextWayPoint)
        {
            Transform tp = _wayPointQueue.Dequeue();
            Debug.Log(tp.name);
            _rb2d.position = tp.position;
            NpcAnimator.SetTrigger("isIdle");
        }
    }

    protected void SetAnimationEnd()
    {
        _isAnimationEnd = true;
    }

    protected void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    public abstract UniTask ExecuteEvent(int index);
    public abstract void FinishEvent(int index);
}
