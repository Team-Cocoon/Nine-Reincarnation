using State;
using State.PlayerState;
using State.StateMachine.PlayerStateMachine;
using UnityEngine;

public enum PlayerDirection
{
    Right = 1,
    Stop = 0,
    Left = -1
}

public class PlayerController : MonoBehaviour
{
    [Header("--- 플레이어 관련 변수 ---")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
    private PlayerDirection _direction; //플레이어 방향
    private Animator _animator;
    private Rigidbody2D _rb2d;
    private PlayerStateMachine _playersStateMachine; //플레이어 상태머신

    public PlayerDirection Direction
    {
        get => _direction;
        set => _direction = value;
    }

    public bool IsGround => _isGround;
    public PlayerStateMachine PlayerStateMachine => _playersStateMachine;

    private void Awake()
    {
        _playersStateMachine = new PlayerStateMachine(this);
        _animator = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();

        _playersStateMachine.stateChanged += ChangeAnimation;
    }

    private void OnDestroy()
    {
        _playersStateMachine.stateChanged -= ChangeAnimation;
    }

    private void FixedUpdate()
    {
        Move();
    }

    //RigidBody를 제어하여 물리적인 움직임을 주는 함수
    private void Move()
    {
        _rb2d.linearVelocityX = (int)_direction * _speed;
    }

    /// <summary>
    /// RigidBody를 제어하여 Jump하는 함수
    /// </summary>
    public void Jump()
    {
        _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
    }

    public void ChangeAnimation(IState state)
    {
        switch ((state as IPlayerState).AnimationState)
        {
            case PlayerAnimationState.Idle:

                break;
            case PlayerAnimationState.Move:

                break;
            case PlayerAnimationState.Jump:

                break;
        }
    }
}
