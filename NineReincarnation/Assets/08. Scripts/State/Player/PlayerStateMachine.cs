namespace State.StateMachine.PlayerStateMachine
{
    public class PlayerStateMachine : StateMachine
    {
        //각 상태들
        public IdleState _idleState;
        public MoveState _moveState;
        public JumpState _jumpState;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="player"></param>
        public PlayerStateMachine(PlayerController player)
        {
            _idleState = new IdleState(player);
            _moveState = new MoveState(player);
            _jumpState = new JumpState(player);
        }
    }
}
