using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    private RaycastOrigins _raycastOrigins;

	private float _horizontalRaySpacing;
	private float _verticalRaySpacing;

    [SerializeField] private float _skinWidth = 0.15f;
    [SerializeField] private int _horizontalRayCount = 3;
	[SerializeField] private int _verticalRayCount = 3;
    [SerializeField] private BoxCollider2D _collider;

    void Start() 
    {
		_collider = GetComponent<BoxCollider2D> ();
	}

	void Update() 
    {
		UpdateRaycastOrigins ();
		CalculateRaySpacing ();

		for (int i = 0; i < _verticalRayCount; i ++) 
        {
			Debug.DrawRay(_raycastOrigins.bottomLeft + Vector2.right * _verticalRaySpacing * i, Vector2.up * -2, Color.red);
		}
	}

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        _raycastOrigins.bottomLeft  = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft     = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight    = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
		Bounds bounds = _collider.bounds;
		bounds.Expand(_skinWidth * -2);

		_horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
		_verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

		_horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
		_verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
	}

}
