using Player.Controller;
using State.PlayerState;

public class MoveState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState { get => _animationState; set => _animationState = value; }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public MoveState(PlayerController player)
    {
        _player = player;
    }
    public void Enter()
    {
        _animationState = PlayerAnimationState.Move;
    }

    public void Execute()
    {
        //공중 상태 진입 시 Jump 싱태로 변환
        if (!_player.IsGround)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._jumpState);
        }
        //플레이이 정지 시 Idle상태로 전환
        else if (_player.Direction == PlayerDirection.Stop)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }
    }

    public void Exit()
    {

    }
}
