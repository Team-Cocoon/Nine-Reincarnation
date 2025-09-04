using Player.Controller;
using State.PlayerState;

public class DeadState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState { get => _animationState; set => _animationState = value; }
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public DeadState(PlayerController player)
    {
        _player = player;
        _animationState = PlayerAnimationState.Dead;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        if (!_player.IsDead)
        {
            _player.IsGround = true;
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }
    }

    public void Exit()
    {
        return;
    }
}
