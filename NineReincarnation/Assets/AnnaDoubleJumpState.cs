using UnityEngine;

public class AnnaDoubleJumpState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        //땅에 닿으면 Idle 상태로 진입
        else if (Player.IsGround || Player.IsSlope)
        {
            animator.SetTrigger("IsIdle");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
