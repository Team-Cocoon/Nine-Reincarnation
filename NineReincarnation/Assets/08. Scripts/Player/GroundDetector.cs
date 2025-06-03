using Player.Controller;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController player;

    private int _groundCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            _groundCount++;
            player.IsGround = true;

            player.ResetJumpCount();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            _groundCount--;
            if(_groundCount == 0)
            {
                player.IsGround = false;
            }
        }
    }
}
