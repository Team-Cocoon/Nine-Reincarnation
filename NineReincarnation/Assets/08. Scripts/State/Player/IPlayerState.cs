namespace State.PlayerState
{
    public enum PlayerAnimationState
    {
        Move,
        Idle,
        Jump,
        Look,
        Dead,
        Throw
    }

    public interface IPlayerState : IState
    {
        public PlayerAnimationState AnimationState { get; }
    }
}

