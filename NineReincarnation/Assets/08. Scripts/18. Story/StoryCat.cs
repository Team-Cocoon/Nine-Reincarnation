using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public class StoryCat : StoryNPC, IEventInterface
{
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private float _speed;

    [SerializeField] private StoryThread _storyThread;
    [SerializeField] private Transform _threadCatPoint;
    [SerializeField] private Transform _threadAnnaPoint;

    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;

    private Queue<Transform> _wayPointQueue;

    private void Awake()
    {
        _wayPointQueue = new Queue<Transform>(_wayPoints);
        _dialogueManager.DialogueEndAddListener(OnDialogueEnd);
    }

    private async UniTask MoveToTarget(bool isRunning)
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

        Vector2 offset = transform.localPosition;
        _rb2d.MovePosition(_rb2d.position + offset);
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
            await RunAndDisappear();
        }
    }

    private async UniTask RunAndDisappear()
    {
        _storyThread.Connect(_threadAnnaPoint, _threadCatPoint);

        _speed = 6.0f;
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CatRun);
        await MoveToTarget(true);
    }

    private void OnDialogueEnd()
    {
        gameObject.SetActive(false);
        _storyThread.Disconnect();
    }
}
