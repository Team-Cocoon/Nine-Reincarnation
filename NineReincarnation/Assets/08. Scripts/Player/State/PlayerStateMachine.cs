using System;
using State.PlayerState;
using Unity.VisualScripting;

namespace State.StateMachine.PlayerStateMachine
{
    public class PlayerStateMachine : StateMachine
    {
        //각 상태들
        public IdleState idleState;
        public MoveState moveState;
        public JumpState jumpState;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="player"></param>
        public PlayerStateMachine(PlayerController player)
        {
            idleState = new IdleState(player);
            moveState = new MoveState(player);
            jumpState = new JumpState(player);
        }
    }
}
