namespace State.PlayerState
{
    public enum PlayerAnimationState
    {
        Move,
        Idle,
        Jump
    }

    public interface IPlayerState : IState
    {
        public PlayerAnimationState AnimationState { get; set; }
    }
}

