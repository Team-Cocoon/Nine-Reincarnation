using EventHandler;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerCamera
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("--- 카메라로 볼 수 있는 최대 거리 ---")]
        [SerializeField] private float _maxLookArea = 10.0f;
        [SerializeField] private float _lookSpeed = 2.0f;
        private CinemachineFollow _cinemachine;

        private void Awake()
        {
            _cinemachine = GetComponent<CinemachineFollow>();
        }

        private void OnEnable()
        {
            CameraEventHandler.OnLook += Look;
        }

        private void OnDisable()
        {
            CameraEventHandler.OnLook -= Look;
        }

        public void Look(bool isLook)
        {
            if (!isLook)
            {
                _cinemachine.FollowOffset.x = 0.0f;
                return;
            }

            if (_cinemachine.FollowOffset.x <= _maxLookArea)
            {
                _cinemachine.FollowOffset.x += Time.deltaTime * _lookSpeed;
            }
        }
    }
}