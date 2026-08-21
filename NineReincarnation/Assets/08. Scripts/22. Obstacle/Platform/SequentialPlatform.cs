using UnityEngine;

public class SequentialPlatform : MonoBehaviour
{
    [SerializeField] private SequentialPlatformChunk _chunk;
    [SerializeField] private int _sequenceIndex = 0;
    [SerializeField] private float activeAlpha = 1.0f;
    [SerializeField] private float inactiveAlpha = 0.3f;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    public bool IsActive { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        SetState(false);
    }

    public int sequenceIndex => _sequenceIndex;
    private void Start()
    {
        var chunk = GetComponentInParent<SequentialPlatformChunk>();
        if (chunk != null) chunk.RegisterSequentialPlatform(this);
    }

    public void SetState(bool isActive)
    {
        IsActive = isActive;

        if (_collider != null) _collider.enabled = isActive;

        if (_renderer != null)
        {
            Color color = _renderer.color;
            color.a = isActive ? activeAlpha : inactiveAlpha;
            _renderer.color = color;
        }
    }
}
