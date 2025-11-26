using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player.Action
{
    public class PlayerAction : MonoBehaviour
    {
        private PlayerController _player;
        public PlayerController Player => _player;

        /// <summary>
        /// 조종할 플레이어 설정
        /// </summary>
        /// <param name="controller"></param>
        public void SetPlayer(PlayerController controller)
        {
            //기존 플레이어 정지 시킴
            _player?.SetStop();

            _player = controller;

            //CameraManager.Instance?.ChangeTarget(controller.GetTransform());
        }

        /// <summary>
        /// 인풋 액션에서 실행시킬 실던지기 함수
        /// </summary>
        /// <param name="context"></param>
        public void ActionThrowThread(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                _player.ExcuteThrowMotion(worldPos);
            }
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

        /// <summary>
        ///  인풋 액션에서 실행시킬 Player Swtich 관련 함수
        /// </summary>
        /// <param name="context"></param>
        public void ActionSwitch(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                //InputManager.Instance.Swap(_playerName);
            }
        }

        /// <summary>
        /// 인풋 액션에서 실행시킬 DownJump관련 함수
        /// </summary>
        public void ActionDownJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _player.DownJump();
            }
        }

        /// <summary>
        /// 인풋 액션에서 실행시킬 Look관련 함수
        /// </summary>
        /// <param name="context"></param>
        public void ActionLook(InputAction.CallbackContext context)
        {
            if (_player.IsDead) return;

            if (context.started)
            {
                _player.IsLook = true;
            }
            else if (context.canceled)
            {
                _player.IsLook = false;
            }
        }
    }
}