using UnityEngine;

public class AnnaJumpState : PlayerStateMachineBehaviour
{
    private int _jumpCount;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Jump;
        _jumpCount = Player.JumpCount;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        else if (Player.IsThrow)
        {
            animator.SetTrigger("IsThrow");
            Player.IsThrow = false;
        }
        //땅에 닿으면 Idle 상태로 진입
        else if (Player.IsGround || Player.IsSlope)
        {
            animator.SetTrigger("IsIdle");
        }
        else if (_jumpCount < Player.JumpCount)
        {
            _jumpCount = Player.JumpCount;
            animator.SetTrigger("IsJump");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
