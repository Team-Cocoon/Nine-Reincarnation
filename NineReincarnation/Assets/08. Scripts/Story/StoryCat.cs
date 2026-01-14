using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public class StoryCat : StoryNPC, IEventInterface
{
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private float _speed;

    private Queue<Transform> _wayPointQueue;


    private void Awake()
    {
        _wayPointQueue = new Queue<Transform>(_wayPoints);
    }

    private async UniTask MoveToTarget()
    {
        if (_wayPoints.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();

        float dist = Vector2.Distance(transform.position, targetTransform.position);

        float sign = Mathf.Sign(targetTransform.position.x - transform.position.x);

        if (sign >= float.Epsilon) Flip(PlayerDirection.Right);
        else Flip(PlayerDirection.Left);

        if (_speed < 4.0f)
        {
            NpcAnimator.SetTrigger("isMove");
        }
        else
        {
            NpcAnimator.SetTrigger("isRun");
        }

        Move(sign);
        while (dist > 0.1f)
        {
            dist = Vector2.Distance(transform.position, targetTransform.position);

            await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate);
        }
        NpcAnimator.SetTrigger("isIdle");

        _rb2d.linearVelocityX = 0.0f;
    }

    private void Move(float direction)
    {
        Debug.Log(direction);

        _rb2d.linearVelocityX = direction * _speed;
    }

    public async UniTask ExecuteEvent(int index)
    {
        if (index == 0)
        {
            await MoveToTarget();
        }
        else if (index == 1)
        {
            _speed = 6.0f;
            await MoveToTarget();
        }
    }
}
