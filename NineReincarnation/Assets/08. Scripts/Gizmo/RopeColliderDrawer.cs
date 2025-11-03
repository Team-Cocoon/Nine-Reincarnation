using UnityEngine;

public class RopeColliderDrawer : MonoBehaviour
{
    public Transform Start;
    public Transform End;

    private void OnDrawGizmos()
    {
        if (Start == null || End == null)
            return;

        Vector2 offset = (Start.position + End.position) / 2;
        Vector2 size = new Vector2(Mathf.Abs(End.position.x - Start.position.x), Mathf.Abs(End.position.y - Start.position.y));

#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
#endif
        Gizmos.DrawLine(Start.position, End.position);      // 내부 면
    }
}
