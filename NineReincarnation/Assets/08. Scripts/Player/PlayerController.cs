using State;
using State.PlayerState;
using State.StateMachine.PlayerStateMachine;
using UnityEngine;

namespace Player.Controller
{
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
        [SerializeField] private string _playerName; //플레이어 식별 변수

        private int _jumpCount = 0; //더블 점프 제어
        private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
        private PlayerDirection _direction; //플레이어 방향
        private Animator _animator;
        private Rigidbody2D _rb2d;
        private PlayerStateMachine _playersStateMachine; //플레이어 상태머신
        private SpriteRenderer _spriteRenderer; //플레이어 이미지

        public PlayerDirection Direction
        {
            get => _direction;
            set => _direction = value;
        }
        public bool IsGround => _isGround;
        public PlayerStateMachine PlayerStateMachine => _playersStateMachine;
        public string PlayerName
        { 
            get => _playerName;
            set => _playerName = value;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playersStateMachine = new PlayerStateMachine(this);
            _animator = GetComponent<Animator>();
            _rb2d = GetComponent<Rigidbody2D>();

            _playersStateMachine.Initialize(_playersStateMachine._idleState);
            _playersStateMachine.stateChanged += ChangeAnimation;
        }

        private void OnDestroy()
        {
            _playersStateMachine.stateChanged -= ChangeAnimation;
        }

        private void Update()
        {
            _playersStateMachine.Excute();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void ResetJumpCount()
        {
            _jumpCount = 0;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        /// <summary>
        /// 플레이어를 정지 상태로 만드는 함수
        /// </summary>
        public void SetStop()
        {
            _direction = PlayerDirection.Stop;
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
            if (_jumpCount >= 2) return;

            _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
            _jumpCount++;
        }

        /// <summary>
        /// 플레이어 방향에 따라 이미지 방향 변경
        /// </summary>
        public void ChangePlayerDirection()
        {
            switch (_direction)
            {
                case PlayerDirection.Right:
                    _spriteRenderer.flipX = false;
                    break;
                case PlayerDirection.Left:
                    _spriteRenderer.flipX = true;
                    break;
            }
        }

        public void ChangeAnimation(IState state)
        {
            switch ((state as IPlayerState).AnimationState)
            {
                case PlayerAnimationState.Idle:
                    _animator.SetTrigger("isIdle");
                    break;
                case PlayerAnimationState.Move:
                    _animator.SetTrigger("isMove");
                    break;
                case PlayerAnimationState.Jump:
                    _animator.SetTrigger("isJump");
                    break;
            }
        }


        #region TODO : 플레이어에 어떤 감지 로직이 더 붙을지 모르므로 나중에 수정필요
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _isGround = true;
            ResetJumpCount();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _isGround = false;
        }
        #endregion
    }
}
