using Player.Controller;
using State.PlayerState;

public class ThrowState : IPlayerState
{
    private PlayerController _player;

    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState => _animationState;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public ThrowState(PlayerController player)
    {
        _player = player;
        _animationState = PlayerAnimationState.Throw;
    }
    public void Enter()
    {

    }

    public void Execute()
    {
        if(!_player.IsThrow)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }
    }

    public void Exit()
    {

    }
}
