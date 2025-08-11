using Player.Controller;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController player;

    private bool isResetJump = false;
    private Collider2D prevCollider;
    private int _groundCount = 0;

    private void Init()
    {
        player.ResetJumpCount();
        isResetJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            _groundCount++;
            player.IsGround = true;
            Init();
        }
        if (collision.CompareTag("Platform"))
        {
            _groundCount++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            if (isResetJump) return;
            if (Mathf.Abs(player.Rb2d.linearVelocityY) > float.Epsilon) return;

            player.IsGround = true;
            Init();
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Platform"))
        {
            _groundCount--;
            if (_groundCount == 0)
            {
                player.IsGround = false;
                isResetJump = false;
            }
        }
    }
}
