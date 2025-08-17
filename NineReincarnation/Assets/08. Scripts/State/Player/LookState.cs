using Player.Controller;
using State.PlayerState;

public class LookState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState { get => _animationState; set => _animationState = value; }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public LookState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Zoom);
        _animationState = PlayerAnimationState.Look;
    }

    public void Execute()
    {
        //Look상태 해제 시 Idle 상태로 변환
        if (!_player.IsLook)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._idleState);
        }

        _player.Look();
    }

    public void Exit()
    {
        return;
    }
}
