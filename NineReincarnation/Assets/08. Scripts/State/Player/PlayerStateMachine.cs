using Player.Controller;

namespace StateMachine.PlayerStateMachine
{
    public class PlayerStateMachine : StateMachine
    {
        //각 상태들
        public IdleState _idleState;
        public MoveState _moveState;
        public JumpState _jumpState;
        public LookState _lookState;
        public DeadState _deadState;

        private PlayerController _player;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="player"></param>
        public PlayerStateMachine(PlayerController player)
        {
            _player = player;

            _idleState = new IdleState(player);
            _moveState = new MoveState(player);
            _jumpState = new JumpState(player);
            _lookState = new LookState(player);
            _deadState = new DeadState(player);
        }

        public override void Excute()
        {
            AnyState();

            base.Excute();
        }


        private void AnyState()
        {
            if (_player.IsDead)
            {
                TransitionTo(_player.PlayerStateMachine._deadState);
            }
        }

    }
}
