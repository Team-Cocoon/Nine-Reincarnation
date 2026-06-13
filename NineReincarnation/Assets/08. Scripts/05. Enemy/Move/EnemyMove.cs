using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Enemy.Move
{
    public class EnemyMove : MonoBehaviour
    {
        public enum WaypointPathType
        {
            Circle, //원
            Ellipse, //타원
            LineClosed, //닫힌 구간
            LineOpen //열린 구간
        }
        [Header("--- 공통 세팅 ---")]
        [SerializeField] private float _spawnTime = 0.0f;
        [SerializeField] private float _delay = 0.0f;
        [SerializeField] private bool isVisible = false;
        [SerializeField] private bool isStopAnimator = false;

        [Header("작동 방식 세팅")]
        [SerializeField] private bool _startOnPlayerStep = false; // true: 플레이어가 밟으면 작동, false: 즉시 작동
        private bool _hasStartedMoving = false; // 중복 실행 방지용 플래그

        [Header("운동 형태 세팅")]
        [SerializeField] private WaypointPathType _pathType = WaypointPathType.LineClosed; //닫힘 => 맨 마지막 웨이포인트와 처음이 이어짐
        [SerializeField] private LoopType _animationLoopType = LoopType.Restart;
        [SerializeField] private int _loopCount = 0;

        [Space(10)]
        [Header("직선 세팅")]
        [SerializeField] private Vector2 _snappingSettings = new Vector2(.1f, .1f); //스냅 단위
        [SerializeField] private bool _editing = false; //editing이 true일때만 웨이포인트 편집 가능
        [SerializeField] private List<Vector3> _waypoints = new List<Vector3>();

        [Space(10)]
        [Header("원 세팅")]
        [SerializeField] private float _circleRadius = 5.0f;
        [SerializeField] private float _angleRad = 0f; //초기 위치
        [SerializeField] private float _circleDirection = 1.0f;

        [Space(10)]
        [Header("타원 세팅")]
        [SerializeField] private float _elipseRadiusX = 5.0f;
        [SerializeField] private float _elipseRadiusY = 10.0f;
        [SerializeField] private float _elipseDirection = 1.0f;

        [Space(10)]
        [Header("에디터 세팅")]
        [SerializeField] private float _handleRadius = .5f; //웨이포인트 원 크기
        [SerializeField] private Color _gizmoDeselectedColor = Color.blue; //선택안된 오브젝트 색

        [Space(10)]
        [Header("움직임 관련 세팅")]
        [SerializeField] private float _duration = 3f; //작동시간
        [SerializeField] private Ease _animaionType = Ease.Linear;
        private Rigidbody2D _rb2d;

        // --- 초기 상태 리셋을 위한 백업 및 제어 변수들 ---
        private Vector3 _origPosition;
        private Vector3 _origLocalPosition;
        private float _origAngleRad;
        private bool _origSpriteEnabled;
        private bool _origColliderEnabled;
        private Tween _activeTween; // 🌟 어떠한 형태의 트윈이든 확실하게 정지시키기 위한 변수

        [Header("---프로퍼티----")]
        public WaypointPathType PathType => _pathType;
        public List<Vector3> Waypoints => _waypoints;
        public Vector2 SnappingSettings => _snappingSettings;
        public bool Editing => _editing;
        public float CircleRadius => _circleRadius;
        public float ElipseRadiusX => _elipseRadiusX;
        public float ElipseRadiusY => _elipseRadiusY;
        public float HandleRadius => _handleRadius;

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
        }

        private void OnValidate()
        {
            if (_waypoints.Count > 0 && (_pathType == WaypointPathType.LineOpen || _pathType == WaypointPathType.LineClosed))
            {
                transform.position = _waypoints[0];
            }
        }

        private void Start()
        {
            // 최초 시작 시점의 원본 데이터들을 안전하게 백업합니다.
            _origPosition = transform.position;
            _origLocalPosition = transform.localPosition;
            _origAngleRad = _angleRad;

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) _origSpriteEnabled = sr.enabled;

            var col = GetComponent<Collider2D>();
            if (col != null) _origColliderEnabled = col.enabled;


            if (isStopAnimator)
            {
                GetComponent<Animator>().Play(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
                GetComponent<Animator>().speed = 0.0f;
            }

            // 플레이어가 밟았을 때 작동하는 모드가 아닐 경우에만 즉시 실행
            if (!_startOnPlayerStep)
            {
                StartMovement();
            }
        }

        // 충돌 감지를 통해 플레이어가 밟았는지 판단
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_startOnPlayerStep && !_hasStartedMoving)
            {
                // 플레이어 태그가 "Player"인지 확인
                if (collision.gameObject.CompareTag("Player"))
                {
                    // 밟았을 때 작동 시작
                    StartMovement();
                }
            }
        }

        // 실제 움직임 시작을 담당하는 메서드
        public void StartMovement()
        {
            if (_hasStartedMoving) return; // 이미 작동 중이면 무시
            _hasStartedMoving = true;

            switch (_pathType)
            {
                case WaypointPathType.Circle:
                    Invoke(nameof(MoveCircle), _spawnTime);
                    break;
                case WaypointPathType.Ellipse:
                    Invoke(nameof(MoveElipse), _spawnTime);
                    break;
                case WaypointPathType.LineClosed:
                    Invoke(nameof(MoveLineClosed), _spawnTime);
                    break;
                case WaypointPathType.LineOpen:
                    Invoke(nameof(MoveLineOpen), _spawnTime);
                    break;
            }
        }

        //원 움직임
        private void MoveCircle()
        {
            Vector3 center = transform.localPosition - new Vector3(0, _circleRadius, 0);
            _angleRad *= Mathf.Deg2Rad;
            
            // 🌟 생성된 트윈을 _activeTween에 할당하여 추적합니다.
            _activeTween = DOTween.To(() => _angleRad,
                                 x => _angleRad = x,
                                 _angleRad - 2f * _circleDirection * Mathf.PI,
                                 _duration)
                .SetEase(Ease.Linear)        // 속도 곡선
                .OnUpdate(() =>
                {
                    Vector3 offset = new Vector3(
                        Mathf.Cos(_angleRad),
                        Mathf.Sin(_angleRad),
                        0f) * _circleRadius;
                    _rb2d.MovePosition(center + offset);
                }).SetLoops(_loopCount, _animationLoopType);
        }

        //타원 움직임
        private void MoveElipse()
        {
            Vector3 center = transform.localPosition - new Vector3(0, _elipseRadiusY, 0);
            _angleRad *= Mathf.Deg2Rad;
            
            // 🌟 생성된 트윈을 _activeTween에 할당하여 추적합니다.
            _activeTween = DOTween.To(() => _angleRad, x => _angleRad = x,
                        _angleRad - 2f * _elipseDirection * Mathf.PI,
                        _duration)
                   .SetEase(Ease.Linear)
                   .OnUpdate(() =>
                   {
                       Vector3 offset = new Vector3(
                           Mathf.Cos(_angleRad) * _elipseRadiusX,
                           Mathf.Sin(_angleRad) * _elipseRadiusY,
                           0f);
                       _rb2d.MovePosition(center + offset);
                   }).SetLoops(_loopCount, _animationLoopType);
        }

        //열린 구간 움직임
        private void MoveLineOpen()
        {
            if (isStopAnimator)
            {
                GetComponent<Animator>().speed = 1.0f;
            }

            if (_waypoints.Count < 1)
            {
                return;
            }

            transform.localPosition = _waypoints[0];
            int wayPointsCount = _waypoints.Count;

            Sequence seq = DOTween.Sequence();

            seq.SetLink(gameObject);
            for (int i = 1; i < wayPointsCount; ++i)
            {
                if (i == 1 || i == wayPointsCount - 1) //처음 목적지
                {
                    seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(_animaionType));
                }
                else //그 외
                {
                    seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(Ease.Linear));
                }
            }

            seq.AppendCallback(() =>
            {
                transform.localPosition = _waypoints[0];
                if (!isVisible)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                    GetComponent<Collider2D>().enabled = false;
                }

                if (isStopAnimator)
                {
                    GetComponent<Animator>().Play(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
                    GetComponent<Animator>().speed = 0.0f;
                }
            });
            seq.AppendInterval(_delay);
            seq.AppendCallback(() =>
            {
                if (!isVisible)
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    GetComponent<Collider2D>().enabled = true;
                }

                if (isStopAnimator)
                {
                    GetComponent<Animator>().speed = 1.0f;
                }
            });
            seq.SetLoops(_loopCount, _animationLoopType);

            // 🌟 시퀀스도 트윈이므로 _activeTween에 담아줍니다.
            _activeTween = seq;
        }

        //닫힌 구간 움직임
        private void MoveLineClosed()
        {
            if (_waypoints.Count < 1)
            {
                return;
            }
            transform.position = _waypoints[0];
            int wayPointsCount = _waypoints.Count;

            Sequence seq = DOTween.Sequence();

            seq.SetLink(gameObject);
            for (int i = 1; i < wayPointsCount; ++i)
            {
                seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(_animaionType));
            }

            seq.Append(_rb2d.DOMove(_waypoints[0], _duration).SetEase(_animaionType));

            seq.AppendInterval(_delay);
            seq.SetLoops(_loopCount, _animationLoopType);

            // 🌟 시퀀스를 _activeTween에 담아줍니다.
            _activeTween = seq;
        }

        // 🌟 플레이어 사망 시 호출되어 버그 없이 완벽히 초기화하는 메서드
        public void ResetToInitialState()
        {
            // 1. 대기 중인 Invoke 및 실행 중이던 모든 형태의 트윈 강제 킬 (가장 중요)
            CancelInvoke();
            if (_activeTween != null)
            {
                _activeTween.Kill();
                _activeTween = null;
            }
            
            // 2. 리지드바디 트윈 정지 및 남아있던 물리 속도(관성)를 완벽히 제로로 초기화
            if (_rb2d != null)
            {
                _rb2d.DOKill();
                _rb2d.linearVelocity = Vector2.zero;
            }
            transform.DOKill();

            // 3. 내부 데이터 변수 원상 복구
            _hasStartedMoving = false;
            _angleRad = _origAngleRad;

            // 4. 물리 위치 캐싱 버그를 막기 위해 transform과 rb2d.position을 동시에 완전히 강제 동기화 리셋
            if (_pathType == WaypointPathType.LineOpen || _pathType == WaypointPathType.LineClosed)
            {
                Vector3 targetPos = _waypoints.Count > 0 ? _waypoints[0] : _origPosition;
                transform.position = targetPos;
                if (_rb2d != null) _rb2d.position = targetPos;
            }
            else
            {
                transform.localPosition = _origLocalPosition;
                if (_rb2d != null) _rb2d.position = transform.position;
            }

            // 5. 비활성화되었을 수 있는 컴포넌트 복구
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = _origSpriteEnabled;

            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = _origColliderEnabled;

            // 6. 애니메이터 상태 복구
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = isStopAnimator ? 0.0f : 1.0f;
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }

            // 7. 즉시 작동 플랫폼이었다면 처음 상태에서 다시 움직이도록 재기동
            if (!_startOnPlayerStep)
            {
                StartMovement();
            }
        }

        private void OnDrawGizmos() //얘는 선택 안되어있을때를 그림
        {
            if (IsSelected() && _editing)
                return;

            switch (_pathType)
            {
                case WaypointPathType.Circle:
                    DrawCircle();
                    break;
                case WaypointPathType.Ellipse:
                    DrawElipse();
                    break;
                case WaypointPathType.LineClosed:
                    DrawLineClosed();
                    break;
                case WaypointPathType.LineOpen:
                    DrawLineOpen();
                    break;
            }
        }

        //타원 그리기
        private void DrawElipse()
        {
            Vector3 centerPosition = transform.localPosition - new Vector3(0, _elipseRadiusY, 0);

            Matrix4x4 prev = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                centerPosition,             // 위치
                Quaternion.identity,       // 회전
                new Vector3(_elipseRadiusX, _elipseRadiusY, 1f) // X, Y만 스케일
            );

            Gizmos.DrawWireSphere(Vector3.zero, 1f);

            Gizmos.matrix = prev; //원상 복구
        }

        //원 그리기
        private void DrawCircle()
        {
            Vector3 centerPosition = transform.localPosition - new Vector3(0, _circleRadius, 0);
            Gizmos.color = _gizmoDeselectedColor;

            Gizmos.DrawWireSphere(centerPosition, _circleRadius);
        }

        //열린 구간 그리기
        private void DrawLineOpen()
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Gizmos.color = _gizmoDeselectedColor;

                Vector3 nextPoint = _waypoints[(i + 1) % _waypoints.Count];
                if (i != _waypoints.Count - 1)
                    Gizmos.DrawLine(_waypoints[i], nextPoint);

                Gizmos.DrawSphere(_waypoints[i], _handleRadius / 2);
            }
        }

        //닫힌 구간 그리기
        private void DrawLineClosed()
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Gizmos.color = _gizmoDeselectedColor;

                Vector3 nextPoint = _waypoints[(i + 1) % _waypoints.Count];
                Gizmos.DrawLine(_waypoints[i], nextPoint);

                Gizmos.DrawSphere(_waypoints[i], _handleRadius / 2);
            }
        }


#if UNITY_EDITOR
        private bool IsSelected()
        {
            return UnityEditor.Selection.activeGameObject == transform.gameObject;
        }
#else
        private bool IsSelected()
        {
            return true;
        }
#endif
    }
}