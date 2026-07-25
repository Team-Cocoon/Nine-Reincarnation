using UnityEngine;

public class AnnaHoldThrowState : PlayerStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.WallHang;
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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Movement input can be held throughout the wall throw, so no new input
        // "started" event is raised after leaving the wall. Restore the facing
        // direction here once the player is no longer attached to the wall.
        if (!Player.IsWallHanging)
        {
            Player.ChangePlayerDirection();
        }

        if (Player.IsWallSliding)
        {
            animator.SetTrigger("IsWallSlide");
        }
    }
}
