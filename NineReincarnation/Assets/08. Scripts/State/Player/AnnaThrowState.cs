using Player.Controller;
using UnityEngine;

public class AnnaThrowState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Throw;
        InputEventHandler.OnChangedForceActionToUI_Invoke();
        Player.SetStop();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.IsDead)
        {
            animator.SetTrigger("IsDead");
            Player.IsDead = false;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InputEventHandler.OnChangedForceActionToPlayer_Invoke();
    }
}
