using UnityEngine;

public class AnnaWallHangState : PlayerStateMachineBehaviour
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
        else if (Player.IsThrow)
        {
            animator.SetTrigger("IsThrow");
            Player.IsThrow = false;
        }
        else if (!Player.IsWallHanging)
        {
            animator.SetTrigger(Player.IsGround ? "IsIdle" : "IsJump");
        }
    }
}
