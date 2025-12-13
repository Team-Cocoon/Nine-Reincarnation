using UnityEngine;

public class ChaseCollider : MonoBehaviour
{
    [SerializeField] private ChaseGhost _chaseGhost;
    [SerializeField] private string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
        {
            _chaseGhost.IsTargetDetected.Value = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
        {
            _chaseGhost.IsTargetDetected.Value = false;
        }
    }
}
