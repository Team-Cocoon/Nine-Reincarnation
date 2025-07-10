using Player.Controller;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController player;

    private int _groundCount = 0;


    private void Init()
    {
        player.IsGround = true;
        _groundCount++;
        player.ResetJumpCount();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slope"))
        {
            if (player.Rb2d.linearVelocityY < 0.1f)
            {
                player.IsSlope = true;
                Init();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            if (player.IsGround)
            {
                return;
            }

            if (Mathf.Abs(player.Rb2d.linearVelocityY) <= 0.1f)
            {
                player.IsGround = true;
                Init();
            }
        }
        else if (collision.CompareTag("Slope"))
        {
            if (player.IsSlope)
            {
                CalculateSlopeVector();
                return;
            }

            if (player.Rb2d.linearVelocityY <= 0.1f)
            {
                player.IsSlope = true;
                Init();
            }
        }
    }

    private void CalculateSlopeVector() //경사진 곳의 경사 벡터를 구함
    {
        Vector2 origin = transform.position;
        float distance = 1.0f;
        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, groundMask);

        if (hit.collider != null)
        {
            Vector2 groundNormal = hit.normal;
            player.SlopeDir = -Vector2.Perpendicular(groundNormal); //반시게 방향으로 90도 회전
            Debug.DrawRay(hit.point, player.SlopeDir, Color.green, 1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Slope"))
        {
            if (collision.CompareTag("Slope"))
            {
                player.IsSlope = false;
                player.SlopeDir = Vector2.right;
            }

            _groundCount--;
            if (_groundCount <= 0)
            {
                _groundCount = 0;
                player.IsGround = false;
            }
        }
    }
}
