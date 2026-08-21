using Player.Controller;
using UnityEngine;

public class SlopeDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController _player;

    private LayerMask _slopeMask;

    private bool _detectedSlope;

    private void Awake()
    {
        _slopeMask = LayerMask.GetMask("Slope");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_player.CurrentState == PlayerAnimationState.Jump)
        {
            return;
        }

        //밑에 경사면 감지
        if (_player.IsSlope)
        {
            //경사면이 있으면 경사면이 ground보다 우선순위
            GetSlopeVector();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        //밑에 경사면 감지
        if (_detectedSlope)
        {
            _player.IsSlope = true;
            _player.IsJump = false;
            _player.ResetJumpCount();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        //밑에 경사면 감지
        if (_detectedSlope)
        {
            _player.IsSlope = false;
        }
    }

    private void GetSlopeVector() //경사진 곳의 경사 벡터를 구함
    {
        Vector2 origin = transform.position;
        float distance = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, _slopeMask);
        Debug.DrawRay(origin, Vector2.down * distance, Color.red, 1f);

        if (hit.collider != null)
        {
            Vector2 groundNormal = hit.normal;
            Debug.DrawRay(hit.point, -Vector2.Perpendicular(groundNormal), Color.green, 1f);
            _player.SlopeDir = -Vector2.Perpendicular(groundNormal); //반시게 방향으로 90도 회전
        }
        else
        {
            _player.SlopeDir = Vector2.right;
        }
    }
}