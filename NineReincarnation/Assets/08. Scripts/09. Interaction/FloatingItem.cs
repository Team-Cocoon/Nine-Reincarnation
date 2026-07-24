using Unity.VisualScripting;
using UnityEngine;

public abstract class FloatingItem : MonoBehaviour
{
    [SerializeField] protected string _playerTag = "Player";

    [SerializeField] private Rigidbody2D _rigid;

    [SerializeField] private float _floatOffset = 1f;
    [SerializeField] private float _floatSpeed = 1f;

    private Vector2 _centerPosition;
    private float _elapsedTime;

    private void OnEnable()
    {
        _centerPosition = transform.position;
        _elapsedTime = 0f;

        _rigid.position = _centerPosition;
    }

    private void FixedUpdate()
    {
        _elapsedTime += Time.fixedDeltaTime;

        float yOffset =
            Mathf.Cos(_elapsedTime * _floatSpeed)
            * (_floatOffset * 0.5f);

        Vector2 nextPosition =
            _centerPosition + Vector2.up * yOffset;

        _rigid.MovePosition(nextPosition);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag) == false) return;

        OnAcquired(collision);
        gameObject.SetActive(false);
    }

    public abstract void OnAcquired(Collider2D collision);
}
