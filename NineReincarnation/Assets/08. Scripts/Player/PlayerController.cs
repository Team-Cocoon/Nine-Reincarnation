using EventHandler;
using Map.Platform;
using UnityEngine;

public enum PlayerAnimationState
{
    Move,
    Idle,
    Jump,
    Look,
    Dead,
    Throw
}

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
        [SerializeField] private float             _defaultGravity;         //상승 중력
        [SerializeField] private float             _defaultDownForce;       //기본 하강시 최대 보정
        [SerializeField] private float             _gliderDownForce;        //글라이딩 하강시 최대 보정
        [SerializeField] private float             _maxDownForce;           //하강시 최대 보정
        [SerializeField] private float             _jumpGravity;            //상승 중력
        [SerializeField] private float             _lighterGravity;         //가벼워 질때 중력
        [SerializeField] private float             _downGravity;            //떨어질때 중력
        [SerializeField] private float             _speed;                  //플레이어 속도
        [SerializeField] private float             _jumpForce;              //점프 힘
        [SerializeField] private string            _playerName;             //플레이어 식별 변수
        [SerializeField] private Vector3           _checkPoint;             //플레이어 리스폰 위치
        [SerializeField] private GroundDetector    _groundDetector;         //땅 감지 
        [SerializeField] private SlopeDetector     _slopeDetector;          //경사면 감지
        [SerializeField] private PhysicsMaterial2D _defaultPhysicsMaterial; //기본 피직스 머티리얼
        [SerializeField] private PhysicsMaterial2D _idlePhysicsMaterial;    //가만히 서 있을때 피직스 머티리얼 
        [SerializeField] private SpriteRenderer    _spriteRenderer;         //플레이어 이미지
        [SerializeField] private ThrowThread       _thread;                 //던질 실

        [Header("--- 플레이어 상태 관련 변수 ---")]
        [SerializeField] private bool _isGround         = false; //플레이어가 땅을 밟고 있는가 판별
        [SerializeField] private bool _isLook           = false; //플레이어가 줌을 실행하고 있는가 판별
        [SerializeField] private bool _isDead           = false; //플레이어가 죽었는가 판별
        [SerializeField] private bool _isSlope          = false;
        [SerializeField] private bool _isJump           = false; //트리거 용
        [SerializeField] private bool _isThrow          = false; //트리거 용
        [SerializeField] private bool _isFalling        = false;
        [SerializeField] private bool _onGroundDetector = false;
        [SerializeField] private bool _onSlopeDetector  = false;

        private int                 _jumpCount = 0;             //더블 점프 제어
        private Vector2             _slopeDir  = Vector2.right; //경사면 이동을 위한 벡터
        private PlayerDirection     _direction;                 //플레이어 방향
        private Animator            _animator;
        private Rigidbody2D         _rb2d;
        private Collider2D          _collider;
        private OneWayPlatform      _oneWayPlatform;
        private PlayerAnimationState _currentState;

#region 프로퍼티 영역
        public int JumpCount => _jumpCount;
        public Rigidbody2D Rb2d => _rb2d;
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
        public bool IsSlope
        {
            get => _isSlope;
            set => _isSlope = value;
        }
        public bool IsJump
        {
            get => _isJump;
            set => _isJump = value;
        }
        public bool IsDead
        {
            get => _isDead;
            set => _isDead = value;
        }
        public bool IsThrow
        {
            get => _isThrow;
            set => _isThrow = value;
        }
        public Vector2 SlopeDir
        {
            get => _slopeDir;
            set => _slopeDir = value;
        }
        public PlayerAnimationState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }
#endregion

        private void Init()
        {
            _isDead           = false;
            _isGround         = false;
            _isSlope          = false;
            _isJump           = false;
            _isFalling        = false;
            _onGroundDetector = false;
            _onSlopeDetector  = false;
            _isThrow          = false;
        }

        private void Awake()
        {
            _rb2d     = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();

            _rb2d.gravityScale = _defaultGravity;

            //모든 상태비헤비어 초기화
            foreach (PlayerStateMachineBehaviour behaviour in _animator.GetBehaviours<PlayerStateMachineBehaviour>())
            {
                behaviour.Player = this;
            }
        }

        private void Start()
        {
            Respawn();
        }

        private void OnDisable()
        {
            AudioManager.Instance.StopLoopingSfx(AudioManager.LoopSfx.Walk);
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            Move();

            UpdateGravityAndFallSpeed();
        }

        private void UpdateGravityAndFallSpeed()
        {
            if (!_isFalling && _rb2d.linearVelocity.y <= 0.5f)
            {
                _rb2d.gravityScale = _downGravity;
                _isFalling = true;
            }
            else if (_isFalling && _rb2d.linearVelocity.y > float.Epsilon)
            {
                _rb2d.gravityScale = _jumpGravity;
                _isFalling = false;
            }

            if (_maxDownForce > _rb2d.linearVelocity.y)
            {
                _rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, _maxDownForce);
            }
        }

        public void ExcuteThrowMotion()
        {
            if (_currentState == PlayerAnimationState.Idle || _currentState == PlayerAnimationState.Move)
            {
                IsThrow = true;
            }
        }
        public void ExcuteThrowThread()
        {
            _thread?.ClickEvent();
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

        public void BecomeLighter()
        {
            _jumpGravity = _lighterGravity;
            _downGravity = _lighterGravity;
            _maxDownForce = _gliderDownForce;
        }

        public void InitGravity()
        {
            _jumpGravity = _defaultGravity;
            _downGravity = _defaultGravity;
            _maxDownForce = _defaultDownForce;
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
            if (_rb2d != null)
            {
                _rb2d.linearVelocityX = 0.0f;
            }
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
        public void IdleEnter()
        {
            _rb2d.sharedMaterial = _idlePhysicsMaterial;
        }
        public void IdleExit()
        {
            _rb2d.sharedMaterial = _defaultPhysicsMaterial;
        }
        public void UpdateGroundDetector(bool isActive)
        {
            if (isActive)
            {
                if (_onGroundDetector)
                {
                    return;
                }

            }
            else
            {
                if (!_onGroundDetector)
                {
                    return;
                }
            }

            _onGroundDetector = isActive;
            _groundDetector.gameObject.SetActive(isActive);
        }

        public void UpdateSlopeDetector(bool isActive)
        {
            if (isActive)
            {
                if (_onSlopeDetector)
                {
                    return;
                }

            }
            else
            {
                if (!_onSlopeDetector)
                {
                    return;
                }
            }

            _onSlopeDetector = isActive;
            _slopeDetector.gameObject.SetActive(isActive);
        }


        //RigidBody를 제어하여 물리적인 움직임을 주는 함수
        private void Move()
        {
            if (_currentState == PlayerAnimationState.Dead) return;
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
            if (_jumpCount >= 2 || _currentState == PlayerAnimationState.Dead) return;

            if(_jumpCount == 0)
            {
                UpdateGroundDetector(false);
                UpdateSlopeDetector(false);

                _isGround = false;
                _isSlope = false;

            }
            _isJump = true;

            _rb2d.linearVelocityY = 0.0f;

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Jump);
            _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
            _jumpCount++;
        }

        /// <summary>
        /// Platform을 제어해서 DownJump하는 함수
        /// </summary>
        public void DownJump()
        {
            if (_oneWayPlatform != null)
            {
                _rb2d.bodyType = UnityEngine.RigidbodyType2D.Dynamic;
                _oneWayPlatform.Ignore(_collider);
            }
        }
        #endregion

        #region 플레이어 상태 제어

        public void Dead()
        {
            if (_currentState == PlayerAnimationState.Dead) return;

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.DIe);
            SetStop();
            _isDead = true;
        }

        public void Respawn()
        {
            Init();
            transform.position = _checkPoint;
        }

        public void Look()
        {
            CameraEventHandler.OnLook_Invoke(_isLook);
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
        #endregion

        #region 충돌 제어

        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool _detectedSlope = ((1 << collision.gameObject.layer) & LayerMask.GetMask("Slope")) != 0;
            bool _detectedGround = ((1 << collision.gameObject.layer) & LayerMask.GetMask("Ground")) != 0;

            if (_detectedSlope)
            {
                UpdateSlopeDetector(true);
                IsJump = false;
            }

            if (_detectedGround)
            {
                if (_rb2d.linearVelocityY <= 0.01f)
                {
                    UpdateGroundDetector(true);
                    IsJump = false;
                }
            }
        }

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
