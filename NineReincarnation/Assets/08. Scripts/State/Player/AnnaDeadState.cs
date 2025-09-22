using UnityEngine;

public class AnnaDeadState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Dead;
        Player.Dead();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Respawn();
    }
}
