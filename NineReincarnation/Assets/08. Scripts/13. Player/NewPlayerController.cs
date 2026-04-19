using UnityEngine;


public class NewPlayerController : MonoBehaviour, IPawnController
{
    private static readonly int IsJumpHash = Animator.StringToHash("IsJump");
    private static readonly int IsMoveHash = Animator.StringToHash("IsMove");
    private static readonly int IsClimbingHash = Animator.StringToHash("IsClimbing");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");


    [Header("----- State------")]
    public bool IsJump;
    public int JumpCount;
    public bool IsMove;
    public bool IsClimbing;
    public bool IsDead;
    public bool IsIdle;
    public bool IsThrow;

    [Header("----- Animation -----")]
    [SerializeField] private Animator _animator;

    [Header("----- Physics -----")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
   

    [Header("----- Smooth Move -----")]
    public float _smoothTime = 0.1f;
    private Vector2 _curVelocity = Vector2.zero;


    public void InitState()
    {
        _animator.SetBool(IsJumpHash, false);
        _animator.SetBool(IsMoveHash, false);
        _animator.SetBool(IsClimbingHash, false);
        _animator.SetBool(IsDeadHash, false);
        _animator.SetBool(IsIdleHash, false);
    }

    public void Update()
    {
        // 애니메이터에 상태 전달
        Animator animator = GetComponent<Animator>();
        animator.SetBool(IsJumpHash, IsJump);
        animator.SetBool(IsMoveHash, IsMove);
        animator.SetBool(IsClimbingHash, IsClimbing);
        animator.SetBool(IsDeadHash, IsDead);
        animator.SetBool(IsIdleHash, IsIdle);
    }
    
    public void Move(int direction)
    {
        
    }

    public void Jump()
    {
        
    }
}
