using Manager.Camera;
using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player.Action
{
    public class PlayerAction : MonoBehaviour
    {
        private string _playerName;
        private PlayerController _player;
        private PlayerInput _playerInput;

        public PlayerController Player => _player;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        /// <summary>
        /// 조종할 플레이어 설정
        /// </summary>
        /// <param name="controller"></param>
        public void SetPlayer(string name, PlayerController controller)
        {
            //기존 플레이어 정지 시킴
            _player?.SetStop();

            _playerName = name;
            _player = controller;

            CameraManager.Instance?.ChangeTarget(controller.GetTransform());
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
                if (_player.DiablePlayerInput()) return;
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
                if (_player.DiablePlayerInput()) return;
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
                if (_player.DiablePlayerInput()) return;
                InputManager.Instance.Swap(_playerName);
            }
        }

        /// <summary>
        /// 인풋 액션에서 실행시킬 DownJump관련 함수
        /// </summary>
        public void ActionDownJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_player.DiablePlayerInput()) return;
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

        /// <summary>
        /// 인풋 액션에서 실행시킬 MainUI관련 함수
        /// </summary>
        /// <param name="context"></param>
        public void ActionToggleMainUI(InputAction.CallbackContext context)
        {
            if (_player.DiablePlayerInput()) return;

            if (context.started)
            {
                //bool isOpen = UIEventHandler.ToggleMainUI();
                //if (isOpen)
                //{
                //    _playerInput.SwitchCurrentActionMap("UI");
                //}
                //else
                //{
                //    _playerInput.SwitchCurrentActionMap("Player");
                //}
            }
        }
    }

}