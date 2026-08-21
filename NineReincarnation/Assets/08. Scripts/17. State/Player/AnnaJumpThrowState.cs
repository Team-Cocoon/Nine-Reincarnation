using UnityEngine;

public class AnnaJumpThrowState : PlayerStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Jump;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        else if (Player.IsGround || Player.IsSlope)
        {
            animator.SetTrigger("IsIdle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!Player.IsWallHanging)
        {
            Player.ChangePlayerDirection();
        }
    }
}
