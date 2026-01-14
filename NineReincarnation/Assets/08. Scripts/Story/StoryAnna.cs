using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public class StoryAnna : StoryNPC, IEventInterface
{
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private float _speed;

    private Queue<Transform> _wayPointQueue;

    private void OnEnable()
    {

    }

    private void Awake()
    {
        _wayPointQueue = new Queue<Transform>(_wayPoints);
    }

    private void OnDestroy()
    {
        AudioManager.Instance?.StopLoopingSfx(AudioManager.LoopSfx.Walk);
    }

    private async UniTask MoveToTarget()
    {
        if (_wayPoints.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();

        float dist = Vector2.Distance(transform.position, targetTransform.position);

        float sign = Mathf.Sign(targetTransform.position.x - transform.position.x);

        if (sign >= float.Epsilon) Flip(PlayerDirection.Right);
        else Flip(PlayerDirection.Left);

        NpcAnimator.SetTrigger("isMove");

        Move(sign);
        AudioManager.Instance?.PlayLoopingSfx(AudioManager.LoopSfx.Walk);
        while (dist > 0.1f)
        {
            dist = Vector2.Distance(transform.position, targetTransform.position);

            await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate);
        }
        NpcAnimator.SetTrigger("isIdle");

        AudioManager.Instance?.StopLoopingSfx(AudioManager.LoopSfx.Walk);
        _rb2d.linearVelocityX = 0.0f;
    }

    public void LoockAroundSoundPlay()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.Look);
    }


    public void SoundPlay()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.Surprised);
    }

    private void Move(float direction)
    {
        Debug.Log(direction);

        _rb2d.linearVelocityX = direction * _speed;
    }

    public async UniTask ExecuteEvent(int index)
    {
        await MoveToTarget();
    }
}
