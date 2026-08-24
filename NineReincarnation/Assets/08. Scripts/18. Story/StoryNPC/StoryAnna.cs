using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Controller;
using UnityEngine;

public class StoryAnna : StoryNPC, IEventInterface
{
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private string _defaultAnimationName = "Anna_Idle";
    [SerializeField] private float _speed;

    private Queue<Transform> _wayPointQueue;

    private void OnEnable()
    {
        NpcAnimator.Play(_defaultAnimationName);
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
        // 수정: 원본 리스트가 아닌 큐에 남은 데이터가 있는지 확인해야 에러가 나지 않습니다.
        if (_wayPointQueue.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();

        NpcAnimator.SetTrigger("isMove");
        AudioManager.Instance?.PlayLoopingSfx(AudioManager.LoopSfx.Walk);

        while (true)
        {
            // 수정: X축으로만 이동하므로 X축 사이의 거리만 계산합니다.
            float distX = Mathf.Abs(targetTransform.position.x - transform.position.x);

            // 수정: 목표 지점에 도달했거나 지나쳤을 경우 루프 탈출
            if (distX <= 0.1f)
            {
                break;
            }

            // 수정: 루프 안에서 방향을 계속 계산하여 목표를 지나치는 것(Overshoot)을 방지합니다.
            float sign = Mathf.Sign(targetTransform.position.x - transform.position.x);

            if (sign >= float.Epsilon) Flip(PlayerDirection.Right);
            else Flip(PlayerDirection.Left);

            // 수정: 마찰력으로 인해 멈추는 것을 방지하기 위해 매 FixedUpdate마다 속도를 줍니다.
            Move(sign);

            await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate);
        }

        NpcAnimator.SetTrigger("isIdle");
        AudioManager.Instance?.StopLoopingSfx(AudioManager.LoopSfx.Walk);
        
        _rb2d.linearVelocityX = 0.0f; // 목표 도달 시 정확히 정지
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
        _rb2d.linearVelocityX = direction * _speed;
    }

    public async UniTask ExecuteEvent(int index)
    {
        await MoveToTarget();
    }

    public void FinishEvent(int index)
    {
        FinishMoveToTarget();
    }

    private void FinishMoveToTarget()
    {
        // 수정: 원본 리스트가 아닌 큐에 남은 데이터가 있는지 확인해야 에러가 나지 않습니다.
        if (_wayPointQueue.Count == 0) return;

        Transform targetTransform = _wayPointQueue.Dequeue();
        NpcAnimator.SetTrigger("isIdle");

        _rb2d.linearVelocityX = 0.0f; // 목표 도달 시 정확히 정지
        _rb2d.position = targetTransform.position;
    }
}