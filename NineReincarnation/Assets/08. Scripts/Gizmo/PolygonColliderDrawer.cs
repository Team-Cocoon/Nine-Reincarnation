using UnityEngine;

[ExecuteAlways]
public class PolygonColliderDrawer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var collider = GetComponent<PolygonCollider2D>();
        if (collider == null || collider.points.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        // PolygonCollider2D는 다각형 path를 여러 개 가질 수 있음
        for (int pathIndex = 0; pathIndex < collider.pathCount; pathIndex++)
        {
            var points = collider.GetPath(pathIndex);
            int count = points.Length;

            for (int i = 0; i < count; i++)
            {
                Vector2 start = transform.TransformPoint(points[i]);
                Vector2 end = transform.TransformPoint(points[(i + 1) % count]);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
