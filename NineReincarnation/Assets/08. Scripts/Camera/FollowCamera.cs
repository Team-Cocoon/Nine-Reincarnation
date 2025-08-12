using EventHandler;
using Manager;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerCamera
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("--- 카메라로 볼 수 있는 최대 거리 ---")]
        [SerializeField] private float _maxLookArea = 10.0f;
        [SerializeField] private float _lookSpeed = 2.0f;
        private CinemachineFollow _followCamera;

        private void Awake()
        {
            CameraManager.Instance.CinemachineCamera = GetComponent<CinemachineCamera>();
            _followCamera = GetComponent<CinemachineFollow>();
        }

        private void OnEnable()
        {
            CameraEventHandler.OnLook += Look;
        }

        private void OnDisable()
        {
            CameraEventHandler.OnLook -= Look;
        }
        private void OnDestroy()
        {
            CameraEventHandler.OnLook -= Look;
        }

        public void Look(bool isLook)
        {
            if (!isLook)
            {
                _followCamera.FollowOffset.x = 0.0f;
                return;
            }

            if (_followCamera.FollowOffset.x <= _maxLookArea)
            {
                _followCamera.FollowOffset.x += Time.deltaTime * _lookSpeed;
            }
        }
    }
}