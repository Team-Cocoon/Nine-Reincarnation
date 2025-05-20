using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;  //에디터 전용 네임스페이스
#endif

namespace Bundos.MovingPlatforms
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
        Loop,
        PingPong
    }

    public class PlatformController : MonoBehaviour
    {
        [Header("웨이 포인트 위치")]
        public List<Vector3> waypoints = new List<Vector3>();

        [Header("원 세팅")]
        public float circleRadius = 5.0f;

        [Header("타원 세팅")]
        public float elipseRadiusX = 5.0f;
        public float elipseRadiusY = 10.0f;

        [Header("에디터 세팅 값")]
        public float handleRadius = .5f;
        public Vector2 snappingSettings = new Vector2(.1f, .1f);
        public Color gizmoDeselectedColor = Color.blue;

        [Header("웨이포인트 세팅")]
        [SerializeField] private Rigidbody2D rb;
        public bool editing = false;

        public WaypointPathType pathType = WaypointPathType.LineClosed;
        public WaypointBehaviorType behaviorType = WaypointBehaviorType.Loop;

        public float moveSpeed = 5f; // Speed of movement
        public float stopDistance = 0.1f; // Distance to consider reaching a waypoint

        private int lastWaypointIndex = -1;
        private int currentWaypointIndex = 0;
        private int direction = 1; // 1 for forward, -1 for reverse

        private void Update()
        {
            if (waypoints.Count == 0)
                return;

            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex]) <= stopDistance)
            {
                if (pathType == WaypointPathType.LineClosed)
                {
                    switch (behaviorType)
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
