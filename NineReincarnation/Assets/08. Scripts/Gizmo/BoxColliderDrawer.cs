using UnityEngine;

[ExecuteAlways]
public class BoxColliderDrawer : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private void OnDrawGizmos()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
            return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // 반투명 초록색
        Vector2 offset = boxCollider.offset;
        Vector2 size = boxCollider.size;

        //BoxCollider2D의 실제 월드 좌표 계산
        Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = matrix;

        Gizmos.DrawCube(offset, size);      // 내부 면
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(offset, size);  // 외곽선
    }
}
