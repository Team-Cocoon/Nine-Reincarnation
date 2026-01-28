using UnityEngine;

public class AnnaLookState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Look;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Zoom);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
        //Look상태 해제 시 Idle 상태로 변환
        else if (Player.IsLook)
        {
            animator.SetTrigger("IsIdle");
        }

        Player.Look();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
