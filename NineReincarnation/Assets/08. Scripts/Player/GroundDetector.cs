using System.Collections.Generic;
using Player.Controller;
using State.PlayerState;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.Image;

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController _player;


    private bool _isSlope = false;
    private LayerMask _groundMask;
    private LayerMask _slopeMask;
    private Vector2 _slopeDir = Vector2.right;
    private ContactFilter2D _filter = new ContactFilter2D();

    private bool _detectedGround;
    private bool _detectedSlope;

    private void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _slopeMask = LayerMask.GetMask("Slope");
    }

    private void Init()
    {
        _player.IsJump = false;
        _player.IsGround = true;
        _player.ResetJumpCount();
    }

    private void OnSlope()
    {
        if (_player.IsJump)
        {
            Debug.Log("1번 조건");
            Debug.Log(_player.Rb2d.linearVelocityY);

            //착지 시
            if (_player.Rb2d.linearVelocityY <= float.Epsilon)
            {
                Init();
                _player.IsSlope = true;
                _player.SlopeDir = _slopeDir;
            }
        }
        //점프 중이 아니라면
        else
        {
            _player.SlopeDir = _slopeDir;
            Init();
            _player.IsSlope = true;
        }
    }

    private void OnGround()
    {
        //땅을 감지했고, 착지 or 가만히 있다면
        if (Mathf.Abs(_player.Rb2d.linearVelocityY) <= 0.01f)
        {
            Init();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;
        _detectedSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        //밑에 경사면 감지
        if (_detectedSlope)
        {
            //경사면이 있으면 경사면이 ground보다 우선순위
            _isSlope = GetSlopeVector();

            if (_isSlope) //레이캐스트까지 충돌 판정 났다면
            {
                OnSlope();
                return;
            }
            else if (_detectedGround)
            {
                _player.Rb2d.linearVelocityY = 0.0f;
                _player.IsSlope = false;
                OnGround();
            }
        }
        else
        {
            if (_player.IsSlope == true)
            {
                _player.Rb2d.linearVelocityY = 0.0f;
            }
            _player.IsSlope = false;
        }

        if (_detectedGround)
        {
            OnGround();
        }
    }

    bool GetSlopeVector() //경사진 곳의 경사 벡터를 구함
    {
        Vector2 origin = transform.position;
        float distance = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, _slopeMask);
        Debug.DrawRay(origin, Vector2.down * distance, Color.red, 1f);

        if (hit.collider != null)
        {
            Vector2 groundNormal = hit.normal;
            Debug.DrawRay(hit.point, -Vector2.Perpendicular(groundNormal), Color.green, 1f);
            _slopeDir = -Vector2.Perpendicular(groundNormal); //반시게 방향으로 90도 회전
            return true;
        }
        else
        {
            _slopeDir = Vector2.right;
            return false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;
        _detectedSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        if (_detectedGround || _detectedSlope)
        {
            _player.IsGround = false;
        }
    }
}
