using System.Collections.Generic;
using Player.Controller;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum DetectorState
{
    None,
    Ground,
    Slope
}

public class GroundDetector : MonoBehaviour
{
    [Header("--- 플레이어 컨트롤러 ---")]
    [SerializeField] private PlayerController _player;

    private DetectorState _detectorState;
    private LayerMask _groundMask;
    private LayerMask _slopeMask;
    private Vector2 _slopeDir = Vector2.right;
    private ContactFilter2D _filter = new ContactFilter2D();

    private void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _slopeMask = LayerMask.GetMask("Slope");
    }

    private void Init()
    {
        _player.IsGround = true;
        _player.ResetJumpCount();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;
        bool isSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        if (isGround)
        {
            if (_detectorState != DetectorState.Ground)
            {
                _player.SlopeDir = Vector2.right;
                _player.IsSlope = false;
                _detectorState = DetectorState.Ground;
            }

            if (_player.IsGround)
            {
                return;
            }

            Debug.Log(_player.Rb2d.linearVelocityY);
            if (Mathf.Abs(_player.Rb2d.linearVelocityY) <= 0.001f)
            {
                _player.SlopeDir = Vector2.right;
                Debug.Log("땅이라 + 1");
                Init();
            }
        }
        
        if (isSlope)
        {
            _player.IsSlope = GetSlopeVector();
            _player.SlopeDir = _slopeDir;

            if (_detectorState == DetectorState.Ground)
            {
                _player.IsSlope = false;
                return;
            }

            if (_detectorState == DetectorState.Slope)
            {
                return;
            }

            if (_player.IsSlope)
            {
                _detectorState = DetectorState.Slope;
                Debug.Log("실이라 + 1");
                Init();
            }
        }
    }

    bool GetSlopeVector() //경사진 곳의 경사 벡터를 구함
    {
        Vector2 origin = transform.position;
        float distance = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, _slopeMask);

        if (hit.collider != null)
        {
            Vector2 groundNormal = hit.normal;
            Debug.DrawRay(hit.point, -Vector2.Perpendicular(groundNormal), Color.green, 1f);
            _slopeDir = - Vector2.Perpendicular(groundNormal); //반시게 방향으로 90도 회전
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
        bool isGround = ((1 << collision.gameObject.layer) & _groundMask) != 0;
        bool isSlope = ((1 << collision.gameObject.layer) & _slopeMask) != 0;

        if (isGround || isSlope)
        {
            _detectorState = DetectorState.None;

            List<Collider2D> results = new List<Collider2D>();
            int count = collision.Overlap(_filter, results);

            _player.IsGround = false;
        }
    }
}
