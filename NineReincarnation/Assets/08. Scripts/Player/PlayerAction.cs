using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    /// <summary>
    /// 인풋 액션에서 실행시킬 Move관련 함수
    /// </summary>
    /// <param name="context"></param>
    public void ActionMove(InputAction.CallbackContext context)
    {
        int direction = (int)context.ReadValue<float>();
        _player.Direction = (PlayerDirection)direction;

        if (context.started)
        {
            _player.ChangePlayerDirection();
        }
    }

    /// <summary>
    /// 인풋 액션에서 실행시킬 Jump관련 함수
    /// </summary>
    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _player.Jump();
        }
    }
}
