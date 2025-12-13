using UnityEngine;

public class InteractionGrass : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _playerTag = "Player";
    private bool _isWobble = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isWobble) return;
        if (!collision.CompareTag(_playerTag)) return;

        _animator.SetTrigger("IsWobble");
        _isWobble = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isWobble) return;
        if (!collision.CompareTag(_playerTag)) return;

        _isWobble = false;
    }
}
