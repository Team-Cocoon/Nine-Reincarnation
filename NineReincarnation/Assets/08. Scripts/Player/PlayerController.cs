using Map.Platform;
using State;
using State.PlayerState;
using State.StateMachine.PlayerStateMachine;
using UnityEngine;

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

        private int _jumpCount = 0; //더블 점프 제어
        [SerializeField] private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
        private PlayerDirection _direction; //플레이어 방향
        private Animator _animator;
        private Rigidbody2D _rb2d;
        private PlayerStateMachine _playersStateMachine; //플레이어 상태머신
        private SpriteRenderer _spriteRenderer; //플레이어 이미지
        private Collider2D _collider;
        private OneWayPlatform _oneWayPlatform;

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

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playersStateMachine = new PlayerStateMachine(this);
            _animator = GetComponent<Animator>();
            _rb2d = GetComponent<Rigidbody2D>();

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
            _rb2d.linearVelocityX = (int)_direction * _speed;
        }

        /// <summary>
        /// RigidBody를 제어하여 Jump하는 함수
        /// </summary>
        public void Jump()
        {
            if (_jumpCount >= 2) return;

            if(_rb2d.linearVelocityY < float.Epsilon)
            {
                _rb2d.linearVelocityY = 0;
            }

            _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
            _jumpCount++;
        }

        /// <summary>
        /// Platform을 제어해여 DownJump하는 함수
        /// </summary>
        public void DownJump()
        {
            _oneWayPlatform?.Ignore(_collider);
        }
        #endregion

        #region 플레이어 상태 제어

        public void Respawn()
        {
            transform.position = _checkPoint;
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
