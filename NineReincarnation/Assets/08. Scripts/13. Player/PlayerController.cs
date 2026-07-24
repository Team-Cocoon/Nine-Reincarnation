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
    WallHang,
    WallSlide,
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
        [SerializeField] private string _playerName;             //플레이어 식별 변수
        [SerializeField] private Vector3 _checkPoint;             //플레이어 리스폰 위치
        [SerializeField] private SpriteRenderer _spriteRenderer;         //플레이어 이미지
        [SerializeField] private ThrowThread[] _thread;                 //던질 실

        [SerializeField, Min(0f)] private float _oneWayDropDuration = 0.25f;
        [SerializeField, Range(0f, 1f)] private float _platformVelocityInheritance = 1f;

        [Header("--- 플레이어 상태 관련 변수 ---")]
        [SerializeField] private bool _isGround = false; //플레이어가 땅을 밟고 있는가 판별
        [SerializeField] private bool _isLook = false; //플레이어가 줌을 실행하고 있는가 판별
        [SerializeField] private bool _isDead = false; //플레이어가 죽었는가 판별
        [SerializeField] private bool _isSlope = false;
        [SerializeField] private bool _isJump = false; //트리거 용
        [SerializeField] private bool _isThrow = false; //트리거 용
        [SerializeField] private bool _isFalling = false;

        [Header("--- 플레이어 실 관련 변수 ---")]
        [SerializeField] private int _currentBlueThread = 3;
        [SerializeField] private int _maxBlueThread = 3;
        private bool _isRedInteract = false;
        private bool _isFeatherFastFall;
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
        private bool _hasPendingThrow;
        private float _groundThrowMoveLockUntil;

        // 클릭 순간의 마우스 위치를 저장할 변수
        private Vector2 _cachedThrowPosition; 

        private float accelerationTimeAirborne = 0.05f;
        private float accelerationTimeGrounded = 0.05f;
        private float velocityXSmoothing;
        
        private Vector2 _velocity;
        private PlatformerRaycastMotor2D _motor;
        private Collider2D _groundCollider;
        private Transform _groundPlatform;
        private Vector2 _groundPlatformPosition;
        private Vector2 _groundPlatformVelocity;
        private float _ignoreOneWayUntil;
        private float _environmentGravityScale;
        private float _environmentDamping;

        // Wall movement values are intentionally constants: they match the design
        // sheet and do not add more per-prefab tuning variables.
        private const float WallHangDelay = 0.5f;
        private const float WallSlideAccelerationTime = 0.3f;
        private const float WallSlideHoldEnd = 2.5f;
        private const float WallSlideStopEnd = 3f;
        private const float WallSlideSpeed = 2f;
        private const float WallJumpCoyoteTime = 0.1f;
        private const float WallJumpBufferTime = 0.1f;
        private const float WallJumpInputLockTime = 0.2f;
        private const float VerticalWallJumpAwaySpeed = 0.1f;

        private bool _isWallHanging;
        private bool _isWallSliding;
        private int _wallDirection;
        private Collider2D _wallCollider;
        private Collider2D _verticalJumpWallCollider;
        private int _sameWallVerticalJumpCount;
        private float _wallHangStartedAt;
        private float _wallCoyoteUntil;
        private float _wallJumpBufferedUntil;
        private float _wallInputIgnoreUntil;
        private int _ignoredWallDirection;

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
        public bool IsFeatherConnected => _isRedInteract;
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
        public float EnvironmentGravityScale
        {
            get => _environmentGravityScale;
            set => _environmentGravityScale = Mathf.Max(0f, value);
        }
        public bool IsWallHanging => _isWallHanging;
        public bool IsWallSliding => _isWallSliding;
        public float EnvironmentDamping
        {
            get => _environmentDamping;
            set => _environmentDamping = Mathf.Max(0f, value);
        }
        #endregion

        private void Init()
        {
            _isDead = false;
            _isGround = false;
            _isSlope = false;
            _isJump = false;
            _isFalling = false;
            _isThrow = false;
            _groundThrowMoveLockUntil = 0f;
            ClearWallHang(false);
            _wallCoyoteUntil = 0f;
            _wallJumpBufferedUntil = 0f;
            _wallInputIgnoreUntil = 0f;
            _sameWallVerticalJumpCount = 0;
            _verticalJumpWallCollider = null;

            _currentBlueThread = _maxBlueThread;

            _velocity = Vector2.zero;
            _environmentGravityScale = _defaultGravity;
            _environmentDamping = 0f;
            ClearGround();

            InitGravity();
        }

        private void OnValidate()
        {
            _checkPoint = transform.position;
        }

        public void ResetVelocityY()
        {
            _velocity.y = 0.0f;
        }

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();

            _motor = GetComponent<PlatformerRaycastMotor2D>();
            if (_motor == null) _motor = gameObject.AddComponent<PlatformerRaycastMotor2D>();
            _rb2d.bodyType = RigidbodyType2D.Kinematic;
            _rb2d.gravityScale = 0f;
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.sharedMaterial = null;
            _rb2d.freezeRotation = true;
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
            _groundThrowMoveLockUntil = 0f;
            ClearWallHang(false);

            ReleaseThreads();

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
        }

        private void FixedUpdate()
        {
            SimulateMotor(Time.fixedDeltaTime);
        }

        private void PrepareThrowMotion(Vector2 mousePosition, ThreadType threadType)
        {
            if (_lockThrow) return;

            // 막는 것이 아니라, 이미 다른 실이 사용 중이라면 강제로 취소(해제)시킵니다.
            for (int i = 0; i < _thread.Length; i++)
            {
                if (i != (int)threadType && _thread[i] != null)
                {
                    if (_thread[i].CurrentState == ThrowThreadState.Exist || _thread[i].CurrentState == ThrowThreadState.Throwing)
                    {
                        _thread[i].ForceCancel(); // 강제로 끊어버리기
                    }
                }
            }

            _pendingThreadType = threadType;
            _cachedThrowPosition = mousePosition; 

            switch (threadType)
            {
                case ThreadType.Red:
                    if (_isRedInteract)
                    {
                        _thread[(int)threadType]?.ClickEvent(_cachedThrowPosition);
                    }
                    else
                    {
                        // 점프 중에도 던지기가 가능하도록 조건 추가
                        if (_currentState == PlayerAnimationState.Idle || _currentState == PlayerAnimationState.Move || _currentState == PlayerAnimationState.Jump)
                        {
                            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
                            float dot = Vector3.Dot(transform.right, playerToMouse);
                            _spriteRenderer.flipX = dot <= float.Epsilon;

                            IsThrow = true;
                            QueueAndExecuteThrow();
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
                        _thread[(int)threadType]?.ClickEvent(_cachedThrowPosition);
                    }
                    else
                    {
                        // 점프 중에도 던지기가 가능하도록 조건 추가
                        if (_currentState == PlayerAnimationState.Idle || _currentState == PlayerAnimationState.Move || _currentState == PlayerAnimationState.Jump)
                        {
                            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
                            float dot = Vector3.Dot(transform.right, playerToMouse);
                            _spriteRenderer.flipX = dot <= float.Epsilon;

                            IsThrow = true;
                            QueueAndExecuteThrow();
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
            if (!_hasPendingThrow) return;
            _hasPendingThrow = false;
            _thread[(int)_pendingThreadType]?.ClickEvent(_cachedThrowPosition);
        }

        private void QueueAndExecuteThrow()
        {
            // Throwing must not depend on an animation event: a double-jump can
            // re-enter the Jump state and consume the transition before that event.
            _animator.ResetTrigger("IsJump");
            _animator.SetTrigger("IsThrow");
            _isThrow = false;

            if (_currentState != PlayerAnimationState.Jump && _isGround)
            {
                _groundThrowMoveLockUntil = Time.time + 0.8f;
                _velocity.x = 0f;
                velocityXSmoothing = 0f;
            }

            _hasPendingThrow = true;
            ExcuteThrowThread();
        }

        public void EndThrowMovementLock()
        {
            _groundThrowMoveLockUntil = 0f;
            ChangePlayerDirection();
        }

        #region 내부 변수 제어
        public void ResetJumpCount() { _jumpCount = 0; }
        public Transform GetTransform() { return transform; }
        public void BecomeLighter() { _isRedInteract = true; _isFeatherFastFall = false; _jumpGravity = _lighterGravity; _downGravity = _lighterGravity; _maxDownForce = _gliderDownForce; }
        public void InitGravity() { _isRedInteract = false; _isFeatherFastFall = false; _jumpGravity = _defaultGravity; _downGravity = _defaultGravity; _maxDownForce = _defaultDownForce; }
        public void SetFeatherFastFall(bool isPressed) { _isFeatherFastFall = isPressed && _isRedInteract; }
        public void SetCheckPoint(Vector3 position) { _checkPoint = position; }
        public void SetStop() { _direction = PlayerDirection.Stop; _velocity.x = 0.0f; }
        public void SetContactPlatform(OneWayPlatform platform = null) { _oneWayPlatform = platform; }
        #endregion

        #region 움직임 관련 부분
        public void IdleEnter() { }
        public void IdleExit() { }
        public void UpdateGroundDetector(bool isActive)
        {
        }
        public void UpdateSlopeDetector(bool isActive)
        {
        }
        private void Move()
        {
            if (_currentState == PlayerAnimationState.Dead) return;
            if (Time.time < _groundThrowMoveLockUntil)
            {
                _velocity.x = 0f;
                velocityXSmoothing = 0f;
                return;
            }

            if (_isWallHanging)
            {
                _velocity.x = 0f;
                velocityXSmoothing = 0f;
                return;
            }

            int moveDirection = (int)_direction;
            if (Time.time < _wallInputIgnoreUntil && moveDirection == _ignoredWallDirection)
                moveDirection = 0;
            float targetVelocityX = moveDirection * _speed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing,
                IsGround ? accelerationTimeGrounded : accelerationTimeAirborne);
        }

        private void SimulateMotor(float deltaTime)
        {
            ApplyPlatformMotion(deltaTime);
            RefreshWallHang();
            Move();

            if (_isWallHanging)
            {
                ApplyWallHangVelocity();
            }
            else if (_motor.Collisions.Below && _velocity.y <= 0f)
            {
                _velocity.y = -2f;
            }
            else
            {
                _isFalling = _velocity.y <= 0f;
                float gravity = (_isFalling ? _downGravity : _jumpGravity) *
                                (_defaultGravity > 0f ? _environmentGravityScale / _defaultGravity : 1f);
                bool fastFalling = _isFalling && _isFeatherFastFall;
                if (fastFalling) gravity *= 4f;
                _velocity.y += Physics2D.gravity.y * gravity * deltaTime;
                _velocity.y = Mathf.Max(_velocity.y, _maxDownForce * (fastFalling ? 4f : 1f));
            }

            if (_environmentDamping > 0f)
                _velocity /= 1f + _environmentDamping * deltaTime;

            bool allowSlopeMovement = _isGround && _direction != PlayerDirection.Stop && _velocity.y <= 0f;
            _motor.Move(_velocity * deltaTime, Time.time < _ignoreOneWayUntil, allowSlopeMovement);
            if (_motor.Collisions.Above || _motor.Collisions.Below) _velocity.y = 0f;
            if ((_motor.Collisions.Left && _velocity.x < 0f) ||
                (_motor.Collisions.Right && _velocity.x > 0f))
            {
                _velocity.x = 0f;
                velocityXSmoothing = 0f;
            }

            _isGround = _motor.Collisions.Below;
            _isSlope = _motor.Collisions.ClimbingSlope || _motor.Collisions.DescendingSlope;
            if (_isGround)
            {
                ClearWallHang(false);
                _wallCoyoteUntil = 0f;
                _isJump = false;
                _jumpCount = 0;
                SetGround(_motor.Collisions.GroundCollider);
            }
            else
            {
                ClearGround();
            }
            RefreshWallHang();
            TryExecuteBufferedWallJump();
            // PlatformerRaycastMotor2D already applies the complete displacement.
            // A velocity on a kinematic Rigidbody would move it once more outside
            // the raycast collision pass and can push the collider into a wall.
            _rb2d.linearVelocity = Vector2.zero;
        }

        private void ApplyPlatformMotion(float deltaTime)
        {
            _groundPlatformVelocity = Vector2.zero;
            if (_groundPlatform == null || !IsGround) return;

            Vector2 current = _groundPlatform.position;
            Vector2 delta = current - _groundPlatformPosition;
            if (deltaTime > 0f) _groundPlatformVelocity = delta / deltaTime;
            _rb2d.position += delta;
            _groundPlatformPosition = current;
        }

        private static bool IsOneWay(Collider2D collider)
        {
            return collider.GetComponentInParent<OneWayPlatform>() != null ||
                   (collider.usedByEffector && collider.GetComponent<PlatformEffector2D>() != null);
        }

        private void SetGround(Collider2D collider)
        {
            _groundCollider = collider;
            Transform platform = collider.attachedRigidbody != null ? collider.attachedRigidbody.transform : collider.transform;
            if (_groundPlatform == platform) return;
            _groundPlatform = platform;
            _groundPlatformPosition = platform.position;
            _oneWayPlatform = collider.GetComponentInParent<OneWayPlatform>();
            collider.GetComponentInParent<Enemy.Move.EnemyMove>()?.TryStartFromPlayerStep();
        }

        private void ClearGround()
        {
            _groundCollider = null;
            _groundPlatform = null;
            _oneWayPlatform = null;
        }

        private void RefreshWallHang()
        {
            if (!HasActiveCatThread())
            {
                CancelWallAbility();
                return;
            }

            bool canHang = !_isGround && !_isDead && _velocity.y <= 0f;
            int inputDirection = (int)_direction;
            Collider2D detectedWall = null;
            int detectedDirection = 0;

            // Only the direction currently held by the player may start or keep
            // a hang. The motor verifies both chest and foot rays on that side.
            if (canHang && inputDirection != 0 && _motor.TryGetVerticalWall(inputDirection, out detectedWall))
            {
                detectedDirection = inputDirection;
            }

            if (detectedDirection != 0)
            {
                bool beganHang = !_isWallHanging;
                _isWallHanging = true;
                _wallDirection = detectedDirection;
                _wallCollider = detectedWall;
                if (beganHang)
                {
                    _wallHangStartedAt = Time.time;
                    _isWallSliding = false;
                    _jumpCount = 0;
                    _animator.SetTrigger("IsWallHang");
                }
                return;
            }

            ClearWallHang(true);
        }

        private void ClearWallHang(bool grantCoyoteTime)
        {
            if (!_isWallHanging) return;
            if (grantCoyoteTime)
                _wallCoyoteUntil = Time.time + WallJumpCoyoteTime;
            _isWallHanging = false;
            _isWallSliding = false;
        }

        private void ApplyWallHangVelocity()
        {
            _velocity.x = 0f;
            float hangTime = Time.time - _wallHangStartedAt;
            if (hangTime < WallHangDelay)
            {
                _velocity.y = 0f;
                return;
            }

            if (!_isWallSliding)
            {
                _isWallSliding = true;
                _animator.SetTrigger("IsWallSlide");
            }

            float slideTime = hangTime - WallHangDelay;
            if (slideTime < WallSlideAccelerationTime)
            {
                _velocity.y = -Mathf.Lerp(0f, WallSlideSpeed, slideTime / WallSlideAccelerationTime);
            }
            else if (slideTime < WallSlideHoldEnd)
            {
                _velocity.y = -WallSlideSpeed;
            }
            else if (slideTime < WallSlideStopEnd)
            {
                float stopT = (slideTime - WallSlideHoldEnd) / (WallSlideStopEnd - WallSlideHoldEnd);
                _velocity.y = -Mathf.Lerp(WallSlideSpeed, 0f, stopT);
            }
            else
            {
                _velocity.y = 0f;
            }
        }

        private void TryExecuteBufferedWallJump()
        {
            if (Time.time > _wallJumpBufferedUntil) return;
            if (!TryExecuteWallJump()) return;
            _wallJumpBufferedUntil = 0f;
        }

        private bool TryExecuteWallJump()
        {
            if (!HasActiveCatThread())
            {
                CancelWallAbility();
                return false;
            }

            bool canUseCurrentWall = _isWallHanging && _wallCollider != null;
            bool canUseCoyoteWall = !canUseCurrentWall && Time.time <= _wallCoyoteUntil && _wallCollider != null;
            if (!canUseCurrentWall && !canUseCoyoteWall) return false;

            int jumpWallDirection = _wallDirection;
            bool verticalJump = (int)_direction == jumpWallDirection;
            _velocity = Vector2.zero;
            _isJump = true;
            _isGround = false;
            _isSlope = false;
            ClearGround();
            ClearWallHang(false);
            _wallCoyoteUntil = 0f;
            _wallJumpBufferedUntil = 0f;
            // A wall jump is the first jump after a wall hold, leaving one
            // regular aerial jump available instead of consuming both counts.
            _jumpCount = 1;

            if (verticalJump)
            {
                if (_verticalJumpWallCollider != _wallCollider)
                {
                    _verticalJumpWallCollider = _wallCollider;
                    _sameWallVerticalJumpCount = 0;
                }

                int decaySteps = Mathf.Min(_sameWallVerticalJumpCount, 3);
                _velocity.x = -jumpWallDirection * VerticalWallJumpAwaySpeed;
                _velocity.y = _jumpForce * 1.2f * Mathf.Pow(0.9f, decaySteps);
                _sameWallVerticalJumpCount++;
                SetFacing(jumpWallDirection);
            }
            else
            {
                _velocity.x = -jumpWallDirection * _jumpForce;
                _velocity.y = _jumpForce * 0.8f;
                _sameWallVerticalJumpCount = 0;
                _verticalJumpWallCollider = null;
                _ignoredWallDirection = jumpWallDirection;
                _wallInputIgnoreUntil = Time.time + WallJumpInputLockTime;
                SetFacing(-jumpWallDirection);
            }

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Jump);
            _animator.SetTrigger("IsJump");
            return true;
        }

        private bool HasActiveCatThread()
        {
            int redThreadIndex = (int)ThreadType.Red;
            if (_thread == null || redThreadIndex >= _thread.Length) return false;

            ThrowThread redThread = _thread[redThreadIndex];
            return redThread != null &&
                   redThread.CurrentState == ThrowThreadState.Exist &&
                   redThread.targetTransform != null &&
                   redThread.targetTransform.TryGetComponent<Cat>(out _);
        }

        private void CancelWallAbility()
        {
            ClearWallHang(false);
            _wallCollider = null;
            _wallDirection = 0;
            _wallCoyoteUntil = 0f;
            _wallJumpBufferedUntil = 0f;
            _wallInputIgnoreUntil = 0f;
            _ignoredWallDirection = 0;
            _sameWallVerticalJumpCount = 0;
            _verticalJumpWallCollider = null;
        }

        public void Jump()
        {
            if (_currentState == PlayerAnimationState.Dead) return;
            if (TryExecuteWallJump()) return;
            if (_jumpCount >= 2)
            {
                _wallJumpBufferedUntil = Time.time + WallJumpBufferTime;
                return;
            }
            if (_jumpCount == 0) { _isGround = false; _isSlope = false; }
            _isJump = true;
            Vector2 inheritedVelocity = _jumpCount == 0 ? _groundPlatformVelocity * _platformVelocityInheritance : Vector2.zero;
            _velocity.y = 0.0f;
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Jump);
            _velocity += inheritedVelocity;
            _velocity.y += _jumpForce;
            ClearGround();
            _jumpCount++;
        }
        public void DownJump()
        {
            if (_oneWayPlatform == null && (_groundCollider == null || !IsOneWay(_groundCollider))) return;
            _ignoreOneWayUntil = Time.time + _oneWayDropDuration;
            _isGround = false;
            _velocity.y = Mathf.Min(_velocity.y, -1f);
            ClearGround();
        }
        #endregion

        #region 플레이어 상태 제어
        public void Dead()
        {
            ReleaseThreads();
            if (_currentState == PlayerAnimationState.Dead) return;
            UpdateGroundDetector(false);
            UpdateSlopeDetector(false);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.DIe);
            SetStop();
            ClearWallHang(false);
            _wallCoyoteUntil = 0f;
            _wallJumpBufferedUntil = 0f;
            _isDead = true;
        }

        private void ReleaseThreads()
        {
            if (_thread == null) return;

            foreach (ThrowThread thread in _thread)
            {
                thread?.ResetThread();
            }
        }

        public void GameEvent_PlayerDead() { GameEventHandler.OnPlayerDead_Invoke(); }
        public void Respawn() { Init(); transform.position = _checkPoint; UpdateGroundDetector(true); UpdateSlopeDetector(true); }
        public void Look() { CameraEventHandler.OnLook_Invoke(_isLook); }
        public void ChangePlayerDirection()
        {
            SetFacing((int)_direction);
        }

        private void SetFacing(int direction)
        {
            switch (direction)
            {
                case 1: _spriteRenderer.flipX = false; break;
                case -1: _spriteRenderer.flipX = true; break;
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
