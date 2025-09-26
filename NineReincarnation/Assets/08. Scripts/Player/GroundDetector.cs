using Player.Controller;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController _player;

    private LayerMask _groundMask;

    private bool _detectedGround;

    private void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;

        if (_detectedGround)
        {
            Debug.Log("땅에 들어옴");
            _player.IsGround = true;
            _player.IsJump = false;
            _player.ResetJumpCount();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;
        if (_detectedGround)
        {
            Debug.Log("땅에서 나감");
            _player.IsGround = false;
        }
    }
}
