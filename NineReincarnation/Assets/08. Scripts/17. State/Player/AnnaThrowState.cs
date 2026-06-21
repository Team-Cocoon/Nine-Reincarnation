using UnityEngine;

public class AnnaThrowState : PlayerStateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.CurrentState = PlayerAnimationState.Throw;
        
        // [수정됨] 실을 던지면서 움직일 수 있도록 아래 두 줄을 주석 처리했습니다.
        // InputEventHandler.OnChangedForceActionToUI_Invoke(); 
        // Player.SetStop(); 
        
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.ThrowThread);
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
        // [수정됨] Enter에서 권한을 뺏지 않았으므로, Exit에서 돌려주는 코드도 주석 처리합니다.
        // InputEventHandler.OnChangedForceActionToPlayer_Invoke();
    }
}