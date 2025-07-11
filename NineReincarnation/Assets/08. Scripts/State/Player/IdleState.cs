using Player.Controller;
using State.PlayerState;

public class IdleState : IPlayerState
{
    private PlayerController _player;
    private PlayerAnimationState _animationState;
    public PlayerAnimationState AnimationState { get => _animationState; set => _animationState = value; }
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="player"></param>
    public IdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _animationState = PlayerAnimationState.Idle;
        _player.Rb2d.bodyType = UnityEngine.RigidbodyType2D.Kinematic;
        _player.Rb2d.linearVelocity = UnityEngine.Vector2.zero;
    }

    public void Execute()
    {
        //공중 상태 진입 시 강제로 Jump 싱태로 변환
        if (!_player.IsGround)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._jumpState);
        }
        //플레이어가 움직이면 Move 상태로 변환
        else if (_player.Direction != PlayerDirection.Stop)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._moveState);
        }
        //플레이어가 Look 상태로 진입하면 상태도 Look으로 변환
        else if (_player.IsLook)
        {
            _player.PlayerStateMachine.TransitionTo(_player.PlayerStateMachine._lookState);
        }
    }

    public void Exit()
    {
        _player.Rb2d.bodyType = UnityEngine.RigidbodyType2D.Dynamic;
    }
}
