using State.PlayerState;

public class JumpState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState { get => _animationState; set => _animationState = value; }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public JumpState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _animationState = PlayerAnimationState.Jump;
    }

    public void Execute()
    {
        //땅에 닿으면 Idle 상태로 진입
        if (_player.IsGround)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }
    }

    public void Exit()
    {

    }
}
