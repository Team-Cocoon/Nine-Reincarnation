using Player.Controller;
using UnityEngine;

public class AnnaThrowState : StateMachineBehaviour
{
    public PlayerController Player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.IsThrow = true;
        InputEventHandler.OnChangedForceActionToUI_Invoke();
        Player.SetStop();
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("호출");
        InputEventHandler.OnChangedForceActionToPlayer_Invoke();
        Player.IsThrow = false;
    }
}
