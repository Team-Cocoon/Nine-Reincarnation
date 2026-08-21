using Player.Controller;
using UnityEngine;

public class AnnaMoveState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Move;
        AudioManager.Instance.PlayLoopingSfx(AudioManager.LoopSfx.Walk);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        //던짐 상태 진입 시 Throw 상태로 변환
        else if (Player.IsThrow)
        {
            animator.SetTrigger("IsThrow");
            Player.IsThrow = false;
        }
        //공중 상태 진입 시 Jump 상태로 변환
        else if (!Player.IsGround && !Player.IsSlope)
        {
            animator.SetTrigger("IsJump");
        }
        //플레이이 정지 시 Idle상태로 전환
        else if (Player.Direction == PlayerDirection.Stop)
        {
            animator.SetTrigger("IsIdle");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Instance.StopLoopingSfx(AudioManager.LoopSfx.Walk);
    }
}
