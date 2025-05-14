using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Status Data")]
    [SerializeField] private Status _statData;

    [Header("Animation")]
    [SerializeField] private AnimState _currentState => _animController.currentState;

    [Header("Movement")]
    [SerializeField] private float _speed;
    private float _horizontalMove;

    [Header("Jumping")]
    [SerializeField] private float _jumpPower;
    private int _jumpCnt;
    private int _maxJumpCnt = 2;

    [Header("GroundCheck")]
    [SerializeField] private bool _isGrounded;
    public Transform groundCheckPos;
    private Vector2 _groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    private Rigidbody2D _rb;
    private PlayerAnimationController _animController;
    //private Animator _animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animController = new PlayerAnimationController(GetComponent<Animator>());
        //_animator = GetComponent<Animator>();
        /* 초기 Stat 설정 */
        _speed = _statData.Speed;
        _jumpPower = _statData.JumpPower;
    }

    void Update()
    {
        GroundCheck();
        FlipSprite();
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_horizontalMove * _speed, _rb.linearVelocity.y);
        _animController.UpdateMovementAnim(_rb.linearVelocity, _isGrounded);
        //_animator.SetFloat("xVelocity", Math.Abs(_rb.linearVelocity.x));
        //_animator.SetFloat("yVelocity", _rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalMove = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_jumpCnt > 1)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower);
                --_jumpCnt;
                //_animator.SetBool("isJumping", true);
                _animController.SetState(AnimState.Jump);
            }
            else if (_jumpCnt > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower * 0.7f);
                --_jumpCnt;
                //_animator.SetBool("isJumping", true);
                _animController.SetState(AnimState.DoubleJump);
            }
        }
    }
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, _groundCheckSize, 0, groundLayer))
        {
            if(_currentState != AnimState.Jump)
            {
                _jumpCnt = _maxJumpCnt;
            }
            _isGrounded = true;
            //_animator.SetBool("isJumping", false);
            //_animController.SetState(AnimState.Idle);
        }
        else
        {
            _isGrounded = false;
        }
    }
    private void FlipSprite()
    {
        if (_horizontalMove > 0) 
        {
            Vector3 vecScale = transform.localScale;
            vecScale.x = 1;
            transform.localScale = vecScale;
        }
        else if (_horizontalMove < 0)
        {
            Vector3 vecScale = transform.localScale;
            vecScale.x = -1;
            transform.localScale = vecScale;
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, _groundCheckSize);
    }
}
