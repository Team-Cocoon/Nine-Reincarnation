using UnityEngine;

public class CameraArea : MonoBehaviour
{
    [Header("--- 카메라 영역 ---")]
    [SerializeField] private PolygonCollider2D _confinerCollider;

    [Header("--- 카메라 ---")]
    [SerializeField] private Camera _camera;

    private float _camHeight, _camWidth;

    private Bounds _colliderBounds;

    private void Awake()
    {
        _camera = Camera.main;
        _camHeight = _camera.orthographicSize;
        _camWidth = _camHeight * _camera.aspect;

        _confinerCollider = GetComponent<PolygonCollider2D>();
        _colliderBounds = _confinerCollider.bounds;
    }

    void LateUpdate()
    {
        _camHeight = _camera.orthographicSize;
        _camWidth = _camHeight * _camera.aspect;

        Vector3 camPos = _camera.transform.position;
        Vector2 clampedPos = ClampCameraPosition(camPos);
        _camera.transform.position = new Vector3(clampedPos.x, clampedPos.y, camPos.z);
    }

    Vector3 ClampCameraPosition(Vector3 position)
    {
        Vector2 min = _colliderBounds.min;
        Vector2 max = _colliderBounds.max;

        // 기본적으로 bounds를 기반으로 1차 제한
        float clampedX = Mathf.Clamp(position.x, min.x + _camWidth, max.x - _camWidth);
        float clampedY = Mathf.Clamp(position.y, min.y + _camHeight, max.y - _camHeight);

        Vector2 clamped = new Vector2(clampedX, clampedY);

        // Collider 내부가 아니면 가장 가까운 지점으로 보정
        if (!_confinerCollider.OverlapPoint(clamped))
        {
            Debug.Log("보정");
            clamped = _confinerCollider.ClosestPoint(clamped);
        }

        return new Vector3(clamped.x, clamped.y, position.z);
    }
}
