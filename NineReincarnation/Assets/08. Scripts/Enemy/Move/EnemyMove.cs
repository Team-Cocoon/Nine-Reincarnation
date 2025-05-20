using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;  //에디터 전용 네임스페이스
#endif

namespace Enemy.Move
{
    public class EnemyMove : MonoBehaviour
    {
        public enum WaypointPathType
        {
            Circle,
            Ellipse,
            LineClosed,
            LineOpen
        }

        public enum WaypointBehaviorType
        {
            Loop, //무한 반복
            PingPong //역전
        }

        [Header("운동 형태 세팅")]
        public WaypointPathType pathType = WaypointPathType.LineClosed; //닫힘 => 맨 마지막 웨이포인트와 처음이 이어짐
        public WaypointBehaviorType behaviorType = WaypointBehaviorType.Loop; //반복 => 계속 반복함

        [Header("직선 세팅")]
        public List<Vector3> waypoints = new List<Vector3>();
        [SerializeField] private Rigidbody2D rb;
        public Vector2 snappingSettings = new Vector2(.1f, .1f); //스냅 단위
        public bool editing = false; //editing이 true일때만 웨이포인트 편집 가능

        [Header("원 세팅")]
        public float circleRadius = 5.0f;

        [Header("타원 세팅")]
        public float elipseRadiusX = 5.0f;
        public float elipseRadiusY = 10.0f;

        [Header("에디터 세팅")]
        public float handleRadius = .5f; //웨이포인트 원 크기
        public Color gizmoDeselectedColor = Color.blue; //선택안된 오브젝트 색

        [Header("움직임 관련 세팅")]
        public float moveSpeed = 5f; //속도
        public float stopDistance = 0.1f; //이 이하면 멈춤

        private int lastWaypointIndex = -1; //마지막 웨이포인트 인덱스
        private int currentWaypointIndex = 0; //현재 웨이포인트 인덱스
        private int direction = 1; // 1은 정방향 -1은 역방향


        private void Update()
        {
            if (waypoints.Count == 0) //웨이포인트가 하나도 없으면 당연히 리턴
                return;

            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex]) <= stopDistance) //현재 위치와 가야할 웨이포인트 간의 거리가 멈춰야할 간격이면
            {
                if (pathType == WaypointPathType.LineClosed)
                {
                    switch (behaviorType) //행동 타입에 따라 결정
                    {
                        case WaypointBehaviorType.Loop:
                            lastWaypointIndex = currentWaypointIndex;
                            currentWaypointIndex = mod((currentWaypointIndex + direction), waypoints.Count);
                            break;
                        case WaypointBehaviorType.PingPong:
                            if ((lastWaypointIndex == 1 && currentWaypointIndex == 0 && direction < 0) || (lastWaypointIndex == waypoints.Count - 1 && currentWaypointIndex == 0 && direction > 0))
                            {
                                direction *= -1;
                            }

                            lastWaypointIndex = currentWaypointIndex;
                            currentWaypointIndex = mod((currentWaypointIndex + direction), waypoints.Count);
                            break;
                    }
                }
                else if (pathType == WaypointPathType.LineOpen)
                {
                    switch (behaviorType)
                    {
                        case WaypointBehaviorType.Loop:
                            int nextWaypointIndex = mod((currentWaypointIndex + direction), waypoints.Count);

                            if ((lastWaypointIndex == 1 && currentWaypointIndex == 0 && direction < 0) || (lastWaypointIndex == waypoints.Count - 2 && currentWaypointIndex == waypoints.Count - 1 && direction > 0))
                            {
                                transform.position = waypoints[nextWaypointIndex];
                            }

                            lastWaypointIndex = currentWaypointIndex;
                            currentWaypointIndex = mod((currentWaypointIndex + direction), waypoints.Count);
                            break;
                        case WaypointBehaviorType.PingPong:
                            if ((lastWaypointIndex == 1 && currentWaypointIndex == 0 && direction < 0) || (lastWaypointIndex == waypoints.Count - 2 && currentWaypointIndex == waypoints.Count - 1 && direction > 0))
                            {
                                direction *= -1;
                            }

                            lastWaypointIndex = currentWaypointIndex;
                            currentWaypointIndex = mod((currentWaypointIndex + direction), waypoints.Count);
                            break;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            MoveToWaypoint(waypoints[currentWaypointIndex]);
        }

        private void MoveToWaypoint(Vector3 waypoint)
        {
            Vector2 direction = (waypoint - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }

        private void OnDrawGizmos() //얘는 선택 안되어있을때를 그림
        {
            if (IsSelected() && editing)
                return;

            switch (pathType)
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

        private void DrawElipse()
        {
            Vector3 centerPosition = transform.position - new Vector3(0, elipseRadiusY, 0);

            Matrix4x4 prev = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                centerPosition,             // 위치
                Quaternion.identity,       // 회전
                new Vector3(elipseRadiusX, elipseRadiusY, 1f) // X, Y만 스케일
            );

            Gizmos.DrawWireSphere(Vector3.zero, 1f);

            Gizmos.matrix = prev; //원상 복구
        }

        private void DrawCircle()
        {
            Vector3 centerPosition = transform.position - new Vector3(0, circleRadius, 0);
            Gizmos.color = gizmoDeselectedColor;

            Gizmos.DrawWireSphere(centerPosition, circleRadius);
        }

        private void DrawLineOpen()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.color = gizmoDeselectedColor;

                Vector3 nextPoint = waypoints[(i + 1) % waypoints.Count];
                if (i != waypoints.Count - 1)
                    Gizmos.DrawLine(waypoints[i], nextPoint);

                Gizmos.DrawSphere(waypoints[i], handleRadius / 2);
            }
        }

        private void DrawLineClosed()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.color = gizmoDeselectedColor;

                Vector3 nextPoint = waypoints[(i + 1) % waypoints.Count];
                Gizmos.DrawLine(waypoints[i], nextPoint);

                Gizmos.DrawSphere(waypoints[i], handleRadius / 2);
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

        int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}