using Player.Controller;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController _player;

    private LayerMask _groundMask;
    private LayerMask _obstacleMask;
    private LayerMask _platformMask;

    private bool _detectedGround;

    private void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _obstacleMask = LayerMask.GetMask("Obstacle");
        _platformMask = LayerMask.GetMask("Platform");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & (_groundMask | _obstacleMask | _platformMask)) != 0;

        if (_detectedGround)
        {
            _player.IsGround = true;
            _player.IsJump = false;
            _player.ResetJumpCount();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & (_groundMask | _obstacleMask | _platformMask)) != 0;
        if (_detectedGround)
        {
            _player.IsGround = false;
        }
    }
}
