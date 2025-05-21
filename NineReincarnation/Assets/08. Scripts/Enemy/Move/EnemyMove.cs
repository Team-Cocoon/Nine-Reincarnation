using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;  //에디터 전용 네임스페이스
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

        [Space(10)]
        [Header("타원 세팅")]
        [SerializeField] private float _elipseRadiusX = 5.0f;
        [SerializeField] private float _elipseRadiusY = 10.0f;

        [Space(10)]
        [Header("에디터 세팅")]
        [SerializeField] private float _handleRadius = .5f; //웨이포인트 원 크기
        [SerializeField] private Color _gizmoDeselectedColor = Color.blue; //선택안된 오브젝트 색

        [Space(10)]
        [Header("움직임 관련 세팅")]
        [SerializeField] private float _duration = 3f; //작동시간
        [SerializeField] private Ease _animaionType = Ease.Linear;
        private Rigidbody2D _rb2d;

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

        private void Start()
        {
            switch (_pathType)
            {
                case WaypointPathType.Circle:
                    MoveCircle();
                    break;
                case WaypointPathType.Ellipse:
                    MoveElipse();
                    break;
                case WaypointPathType.LineClosed:
                    MoveLineClosed();
                    break;
                case WaypointPathType.LineOpen:
                    MoveLineOpen();
                    break;
            }

        }

        //원 움직임
        private void MoveCircle()
        {
            Vector3 center = transform.position - new Vector3(0, _circleRadius, 0);

            DOTween.To(() => _angleRad,
                                 x => _angleRad = x,
                                 _angleRad - 2f * Mathf.PI,
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
            float angleRad = 0f; //현재 각도
            Vector3 center = transform.position - new Vector3(0, _elipseRadiusY, 0);
            DOTween.To(() => angleRad, x => angleRad = x, 
                        angleRad - 2f * Mathf.PI, 
                        _duration)
                   .SetEase(Ease.Linear)
                   .OnUpdate(() => {
                       Vector3 offset = new Vector3(
                           Mathf.Cos(angleRad) * _elipseRadiusX,
                           Mathf.Sin(angleRad) * _elipseRadiusY,
                           0f);
                       _rb2d.MovePosition(center + offset);
                   }).SetLoops(_loopCount, _animationLoopType);
        }

        //닫힌 구간 움직임
        private void MoveLineOpen()
        {
            if(_waypoints.Count < 1)
            {
                return;
            }
            transform.position = _waypoints[0];
            int wayPointsCount = _waypoints.Count;
            Sequence seq = DOTween.Sequence();

            for(int i = 1; i < wayPointsCount; ++i)
            {
                if(i == 1 || i == wayPointsCount - 1) //처음 목적지
                {
                    seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(_animaionType));
                }
                else //그 외
                {
                    seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(Ease.Linear));
                }
            }

            seq.SetLoops(_loopCount, _animationLoopType);
        }

        private void MoveLineClosed()
        {
            if (_waypoints.Count < 1)
            {
                return;
            }
            transform.position = _waypoints[0];
            int wayPointsCount = _waypoints.Count;
            Sequence seq = DOTween.Sequence();

            for (int i = 1; i < wayPointsCount; ++i)
            {
                 seq.Append(_rb2d.DOMove(_waypoints[i], _duration).SetEase(Ease.Linear));
            }

            seq.Append(_rb2d.DOMove(_waypoints[0], _duration).SetEase(Ease.Linear));

            seq.SetLoops(_loopCount, _animationLoopType);
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
            Vector3 centerPosition = transform.position - new Vector3(0, _elipseRadiusY, 0);

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
            Vector3 centerPosition = transform.position - new Vector3(0, _circleRadius, 0);
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