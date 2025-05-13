using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Status Data")]
    [SerializeField] private Status statData;

    [Header("Movement")]
    [SerializeField] private float _speed;
    private float _horizontalMove;

    [Header("Jumping")]
    [SerializeField] private float _jumpPower;
    private int jumpCnt;
    private int maxJumpCnt = 2;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;


    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        /* 초기 Stat 설정 */
        _speed = statData.Speed;
        _jumpPower = statData.JumpPower;
    }

    void Update()
    {
        GroundCheck();
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_horizontalMove * _speed, _rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalMove = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (jumpCnt > 1)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower);
                Debug.Log($"JumpCnt: {jumpCnt}");
                --jumpCnt;
            }
            else if (jumpCnt > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower * 0.7f);
                Debug.Log($"JumpCnt: {jumpCnt}");
                --jumpCnt;
            }
        }
    }
    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpCnt = maxJumpCnt;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
