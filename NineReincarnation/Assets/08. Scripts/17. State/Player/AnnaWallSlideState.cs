using UnityEngine;

public class AnnaWallSlideState : PlayerStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.WallSlide;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        else if (!Player.IsWallHanging)
        {
            animator.SetTrigger(Player.IsGround ? "IsIdle" : "IsJump");
        }
    }
}
