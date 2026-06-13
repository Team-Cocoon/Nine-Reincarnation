using System;
using Cysharp.Threading.Tasks;
using EventHandler;
using Map.Platform;
using UnityEngine;
using VContainer;

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
        [SerializeField] private float _defaultGravity;         //상승 중력
        [SerializeField] private float _defaultDownForce;       //기본 하강시 최대 보정
        [SerializeField] private float _gliderDownForce;        //글라이딩 하강시 최대 보정
        [SerializeField] private float _maxDownForce;           //하강시 최대 보정
        [SerializeField] private float _jumpGravity;            //상승 중력
        [SerializeField] private float _lighterGravity;         //가벼워 질때 중력
        [SerializeField] private float _downGravity;            //떨어질때 중력
        [SerializeField] private float _speed;                  //플레이어 속도
        [SerializeField] private float _jumpForce;              //점프 힘
        [SerializeField] private float _enhancedJumpForce;      // ✨ 추가: 고양이 상호작용 시 강화될 점프 힘
        [SerializeField] private string _playerName;             //플레이어 식별 변수
        [SerializeField] private Vector3 _checkPoint;             //플레이어 리스폰 위치
        [SerializeField] private GroundDetector _groundDetector;         //땅 감지 
        [SerializeField] private SlopeDetector _slopeDetector;          //경사면 감지
        [SerializeField] private PhysicsMaterial2D _defaultPhysicsMaterial; //기본 피직스 머티리얼
        [SerializeField] private PhysicsMaterial2D _idlePhysicsMaterial;    //가만히 서 있을때 피직스 머티리얼 
        [SerializeField] private SpriteRenderer _spriteRenderer;         //플레이어 이미지
        [SerializeField] private ThrowThread[] _thread;                 //던질 실

        [Header("--- 플레이어 상태 관련 변수 ---")]
        [SerializeField] private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
        [SerializeField] private bool _isLook = false; //플레이어가 줌을 실행하고 있는가 판별
        [SerializeField] private bool _isDead = false; //플레이어가 죽었는가 판별
        [SerializeField] private bool _isSlope = false;
        [SerializeField] private bool _isJump = false; //트리거 용
        [SerializeField] private bool _isThrow = false; //트리거 용
        [SerializeField] private bool _isFalling = false;
        [SerializeField] private bool _onGroundDetector = false;
        [SerializeField] private bool _onSlopeDetector = false;

        [Header("--- 플레이어 실 관련 변수 ---")]
        [SerializeField] private int _currentBlueThread = 3;
        [SerializeField] private int _maxBlueThread = 3;
        private bool _isRedInteract = false;
        private bool _isBlueInteract = false;
        private int _activePhasingCount = 0;

        private int _jumpCount = 0;             //더블 점프 제어
        private Vector2 _slopeDir = Vector2.right; //경사면 이동을 위한 벡터
        private PlayerDirection _direction;                 //플레이어 방향
        private Animator _animator;
        private Rigidbody2D _rb2d;
        private Collider2D _collider;
        private OneWayPlatform _oneWayPlatform;
        private PlayerAnimationState _currentState;
        private bool _lockThrow = true;
        private ThreadType _pendingThreadType;

        private float accelerationTimeAirborne = 0.05f;
        private float accelerationTimeGrounded = 0.05f;
        private float velocityXSmoothing;
        
        private float _baseJumpForce; // ✨ 추가: 원래 점프 힘을 저장해둘 내부 변수

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
        public int BlueThread
        {
            get => _currentBlueThread;
            set {
                _currentBlueThread = (_currentBlueThread - 1 < 0) ? 0 : _currentBlueThread - 1;
            }
        }
        public int ActivePhasingCount => _activePhasingCount;
        #endregion

        private void Init()
        {
            _isDead = false;
            _isGround = false;
            _isSlope = false;
            _isJump = false;
            _isFalling = false;
            _isThrow = false;

            _currentBlueThread = _maxBlueThread;

            _onGroundDetector = false;
            _onSlopeDetector = false;

            InitGravity();
            InitJumpForce(); // ✨ 추가: 리스폰 시 점프력도 초기 상태로 복구
        }

        private void OnValidate()
        {
            _checkPoint = transform.position;
        }

        public void ResetVelocityY()
        {
            _rb2d.linearVelocityY = 0.0f;
        }

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();

            _rb2d.gravityScale = _defaultGravity;
            _baseJumpForce = _jumpForce; // ✨ 추가: 인스펙터에 세팅된 기본 점프 힘 백업
        }

        private void OnEnable()
        {
            SetLockThrow().Forget();

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
            if (!_lockThrow) _lockThrow = true;

            AudioManager.Instance.StopLoopingSfx(AudioManager.LoopSfx.Walk);
        }

        private void Update()
        {

        }

        private async UniTaskVoid SetLockThrow()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());

            if (this == null || !this.isActiveAndEnabled) return;
            _lockThrow = false;

            Debug.Log("던지기 실행가능");
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

        private void PrepareThrowMotion(Vector2 mousePosition, ThreadType threadType)
        {
            if (_lockThrow) return;

            _pendingThreadType = threadType;
            switch (threadType)
            {
                case ThreadType.Red:
                    if (_isRedInteract)
                    {
                        _thread[(int)threadType]?.ClickEvent();
                    }
                    else
                    {
                        if (_currentState == PlayerAnimationState.Idle || _currentState == PlayerAnimationState.Move)
                        {
                            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
                            float dot = Vector3.Dot(transform.right, playerToMouse);
                            _spriteRenderer.flipX = dot <= float.Epsilon;

                            IsThrow = true;
                        }
                    }
                    break;
                case ThreadType.Blue:
                    if (_currentBlueThread == 0 || _activePhasingCount > 0) return;

                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, LayerMask.GetMask("Interaction"));
                    bool shouldClick = false;
                    if (hit.collider != null)
                    {
                        var phasable = hit.collider.GetComponent<IPhasable>();
                        if (phasable != null)
                        {
                            if (phasable.IsConnected)
                            {
                                return;
                            }
                        }
                    }
                    if (shouldClick)
                    {
                        _thread[(int)threadType]?.ClickEvent();
                    }
                    else
                    {
                        if (_currentState == PlayerAnimationState.Idle || _currentState == PlayerAnimationState.Move)
                        {
                            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
                            float dot = Vector3.Dot(transform.right, playerToMouse);
                            _spriteRenderer.flipX = dot <= float.Epsilon;

                            IsThrow = true;
                        }
                    }
                    break;
            }
        }
        
        public void AddActivePhasing() => _activePhasingCount++;
        public void RemoveActivePhasing() => _activePhasingCount = Mathf.Max(0, _activePhasingCount - 1);
        public void ExcuteRedThrowMotion(Vector2 mousePosition) => PrepareThrowMotion(mousePosition, ThreadType.Red);
        public void ExcuteBlueThrowMotion(Vector2 mousePosition) => PrepareThrowMotion(mousePosition, ThreadType.Blue);

        public void ExcuteThrowThread()
        {
            _thread[(int)_pendingThreadType]?.ClickEvent();
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
            _isRedInteract = true;
            _jumpGravity = _lighterGravity;
            _downGravity = _lighterGravity;
            _maxDownForce = _gliderDownForce;
        }

        public void InitGravity()
        {
            _isRedInteract = false;
            _jumpGravity = _defaultGravity;
            _downGravity = _defaultGravity;
            _maxDownForce = _defaultDownForce;
        }

        // ✨ 추가: 점프력 강화 메서드 (고양이 상호작용용)
        public void EnhanceJump()
        {
            _isBlueInteract = true; // 파란 실 상호작용 판정 플래그 ON (필요에 따라 수정 가능)
            _jumpForce = _enhancedJumpForce;
        }

        // ✨ 추가: 점프력 초기화 메서드
        public void InitJumpForce()
        {
            _isBlueInteract = false;
            _jumpForce = _baseJumpForce;
        }

        public void SetCheckPoint(Vector3 position)
        {
            _checkPoint = position;
        }

        public void SetStop()
        {
            _direction = PlayerDirection.Stop;
            if (_rb2d != null)
            {
                _rb2d.linearVelocityX = 0.0f;
            }
        }

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
                if (_onGroundDetector) return;
            }
            else
            {
                if (!_onGroundDetector) return;
            }

            _onGroundDetector = isActive;
            _groundDetector.gameObject.SetActive(isActive);
        }

        public void UpdateSlopeDetector(bool isActive)
        {
            if (isActive)
            {
                if (_onSlopeDetector) return;
            }
            else
            {
                if (!_onSlopeDetector) return;
            }

            _onSlopeDetector = isActive;
            _slopeDetector.gameObject.SetActive(isActive);
        }

        private void Move()
        {
            if (_currentState == PlayerAnimationState.Dead) return;
            if (IsSlope)
            {
                _rb2d.linearVelocity = (int)_direction * _speed * _slopeDir;
            }
            else
            {
                float targetVelocityX = (int)_direction * _speed;
                _rb2d.linearVelocityX = Mathf.SmoothDamp(_rb2d.linearVelocityX, targetVelocityX, ref velocityXSmoothing, (IsGround) ? accelerationTimeGrounded : accelerationTimeAirborne);
            }
        }

        public void Jump()
        {
            if (_jumpCount >= 2 || _currentState == PlayerAnimationState.Dead) return;

            if (_jumpCount == 0)
            {
                _isGround = false;
                _isSlope = false;
            }
            _isJump = true;

            _rb2d.linearVelocityY = 0.0f;

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Jump);
            _rb2d.AddForceY(_jumpForce, ForceMode2D.Impulse);
            _jumpCount++;
        }

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

            UpdateGroundDetector(false);
            UpdateSlopeDetector(false);

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.DIe);
            SetStop();
            _isDead = true;
        }

        public void GameEvent_PlayerDead()
        {
            GameEventHandler.OnPlayerDead_Invoke();
        }

        public void Respawn()
        {
            Init();
            transform.position = _checkPoint;
            UpdateGroundDetector(true);
            UpdateSlopeDetector(true);
        }

        public void Look()
        {
            CameraEventHandler.OnLook_Invoke(_isLook);
        }

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