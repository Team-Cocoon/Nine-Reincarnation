using Player.Controller;
using UnityEngine;

public class AnnaIdleState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Idle;
        Player.IdleEnter();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        else if (Player.IsThrow)
        {
            animator.SetTrigger("IsThrow");
            Player.IsThrow = false;
        }
        //공중 상태 진입 시 강제로 Jump 상태로 변환
        else if (!(Player.IsGround || Player.IsSlope) || Player.IsJump)
        {
            animator.SetTrigger("IsJump");
            Player.IsJump = false;
        }
        //플레이어가 움직이면 Move 상태로 변환
        else if (Player.Direction != PlayerDirection.Stop)
        {
            animator.SetTrigger("IsMove");
        }
        //플레이어가 Look 상태로 진입하면 상태도 Look으로 변환
        else if (Player.IsLook)
        {
            animator.SetTrigger("IsLook");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.IdleExit();
    }
}
