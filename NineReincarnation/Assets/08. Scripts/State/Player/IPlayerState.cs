namespace State.PlayerState
{
    public enum PlayerAnimationState
    {
        Move,
        Idle,
        Jump,
        Look,
        Dead
    }

    public interface IPlayerState : IState
    {
        public PlayerAnimationState AnimationState { get; }
    }
}

