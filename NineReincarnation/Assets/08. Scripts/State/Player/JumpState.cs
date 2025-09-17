using Player.Controller;
using State.PlayerState;

public class JumpState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState => _animationState;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public JumpState(PlayerController player)
    {
        _player = player;
        _animationState = PlayerAnimationState.Jump;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        //땅에 닿으면 Idle 상태로 진입
        if (_player.IsGround || _player.IsSlope)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }
    }

    public void Exit()
    {

    }
}
