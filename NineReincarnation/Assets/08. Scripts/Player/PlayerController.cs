using EventHandler;
using Map.Platform;
using State;
using State.PlayerState;
using State.StateMachine.PlayerStateMachine;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IObjectData
{
    public float Speed
    {
        get;
        set;
    }
}

namespace Player.Controller
{
    public enum PlayerDirection
    {
        Right = 1,
        Stop = 0,
        Left = -1
    }
    public class PlayerController : MonoBehaviour, IObjectData
    {
        [Header("--- 플레이어 관련 변수 ---")]
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private string _playerName; //플레이어 식별 변수
        [SerializeField] private Vector3 _checkPoint; //플레이어 리스폰 위치
        [SerializeField] private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
        [SerializeField] private bool _isLook = false; //플레이어가 줌을 실행하고 있는가 판별
        [SerializeField] private bool _isDead = false; //플레이어가 죽었는가 판별
        [SerializeField] private bool _isBusy = false;
        [SerializeField] private bool _isSlope = false;

        private int _jumpCount = 0; //더블 점프 제어
        private PlayerDirection _direction; //플레이어 방향
        private Animator _animator;
        private Rigidbody2D _rb2d;
        private PlayerStateMachine _playersStateMachine; //플레이어 상태머신
        private SpriteRenderer _spriteRenderer; //플레이어 이미지
        private Collider2D _collider;
        private OneWayPlatform _oneWayPlatform;
        private PlayerAnimationState _currentState;
        private Vector2 _slopeDir = Vector2.right; //경사면 이동을 위한 벡터

        public int JumpCount => _jumpCount;
        public PlayerAnimationState CurrentState => _currentState;
        public Rigidbody2D Rb2d => _rb2d;

        public bool IsDead => _isDead;

        public bool IsLook
        {
            get => _isLook;
            set => _isLook = value;
        }
        public PlayerDirection Direction
        {
            get => _direction;
            set => _direction = value;
        }
        public bool IsGround
        {
            get => _isGround;
            set => _isGround = value;
        }
        public PlayerStateMachine PlayerStateMachine => _playersStateMachine;
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public Vector3 CheckPoint
        {
            get => _checkPoint;
            set => _checkPoint = value;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => _isBusy = value;
        }

        public bool IsSlope
        {
            get => _isSlope;
            set => _isSlope = value;
        }

        public Vector2 SlopeDir
        {
            get => _slopeDir;
            set => _slopeDir = value;
        }

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playersStateMachine = new PlayerStateMachine(this);
            _animator = GetComponent<Animator>();

            _playersStateMachine.Initialize(_playersStateMachine._idleState);
            _playersStateMachine.stateChanged += ChangeAnimation;
        }

        private void Start()
        {
            Respawn();
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

        public bool DiablePlayerInput()
        {
            return _isLook || _isDead || _isBusy;
        }

        #region 내부 변수 제어
        public void ResetJumpCount()
        {
            _jumpCount = 0;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void SetCheckPoint(Vector3 position)
        {
            _checkPoint = position;
        }

        /// <summary>
        /// 플레이어를 정지 상태로 만드는 함수
        /// </summary>
        public void SetStop()
        {
            _direction = PlayerDirection.Stop;
            _rb2d.linearVelocityX = 0.0f;
        }

        /// <summary>
        /// 현재 접촉한 OneWayPlatform 설정
        /// </summary>
        public void SetContactPlatform(OneWayPlatform platform = null)
        {
            _oneWayPlatform = platform;
        }
        #endregion

        #region 움직임 관련 부분

        //RigidBody를 제어하여 물리적인 움직임을 주는 함수
        private void Move()
        {
            if (_isDead) return;
            if (_isBusy)
            {
                SetStop();
            }

            if (IsSlope)
            {
                _rb2d.linearVelocity = (int)_direction * _speed * _slopeDir;
            }
            else
            {
                _rb2d.linearVelocityX = (int)_direction * _speed;
            }
        }

        /// <summary>
        /// RigidBody를 제어하여 Jump하는 함수
        /// </summary>
        public void Jump()
        {
            if (_jumpCount >= 2) return;

            _isGround = false;
            _isSlope = false;
            _rb2d.linearVelocityY = 0.0f;

            _rb2d.bodyType = UnityEngine.RigidbodyType2D.Dynamic;
            AudioManger.Instance.PlaySfx(AudioManger.Sfx.Jump);
            _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
            _jumpCount++;
        }

        /// <summary>
        /// Platform을 제어해여 DownJump하는 함수
        /// </summary>
        public void DownJump()
        {
            if (_oneWayPlatform != null)
            {
                _rb2d.bodyType = UnityEngine.RigidbodyType2D.Dynamic;
                _oneWayPlatform.Ignore(_collider);
                //_isGround = false;
            }
        }
        #endregion

        #region 플레이어 상태 제어

        public void Dead()
        {
            if (_isDead) return;

            AudioManger.Instance.PlaySfx(AudioManger.Sfx.DIe);
            SetStop();
            _isDead = true;
        }

        public void Respawn()
        {
            _isDead = false;
            transform.position = _checkPoint;
        }

        public void Look()
        {
            CameraEventHandler.OnLook(_isLook);
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
            _currentState = (state as IPlayerState).AnimationState;
            switch (_currentState)
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
                case PlayerAnimationState.Look:
                    _animator.SetTrigger("isLook");
                    break;
                case PlayerAnimationState.Dead:
                    _animator.SetTrigger("isDead");
                    break;
            }
        }
        #endregion

        #region 충돌 제어
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
            collidable?.Enter(gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
            collidable?.Exit(gameObject);
        }
        #endregion
    }
}
