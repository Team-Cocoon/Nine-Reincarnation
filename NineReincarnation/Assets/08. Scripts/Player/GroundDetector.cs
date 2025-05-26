using Player.Controller;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            player.IsGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            player.IsGround = false;
        }
    }
}
