using Unity.VisualScripting;
using UnityEngine;

public interface ISink
{
    public void Sink();
    public void Exit();
}

public class Swamp : MonoBehaviour, ISink
{
    [Header("Sink Variable")]
    [SerializeField] private float sinkSpeed = 0.1f;
    [SerializeField] private float minSize = 0f;
    [SerializeField] private bool _isSink;

    private BoxCollider2D _collider2D;
    private Vector2 _originPos;
    private Vector2 _originSize;

    private void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        _originPos = transform.position;
        _originSize = _collider2D.size;
    }
    private void Update()
    {
        float variable = sinkSpeed * Time.deltaTime;
        Vector2 size = _collider2D.size;

        if (_isSink)
        {
            size.y -= variable;
            if (size.y < minSize) return; // 죽는 처리 하던지
            _collider2D.size = size;
            transform.Translate(Vector2.down * variable);
        }
        else if(size.y >= _originSize.y)
        {
            _collider2D.size = _originSize;
        }
        else
        {
            size.y += variable;
            _collider2D.size = size;
            transform.position = Vector2.MoveTowards(transform.position, _originPos, variable);
        }
    }
    
    public void Sink()
    {
        _isSink = true;
    }
    public void Exit()
    {
        _isSink = false;
    }
}
