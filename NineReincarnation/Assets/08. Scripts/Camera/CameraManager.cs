using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Manager
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [SerializeField] private Camera _uiCamera;

        private CinemachineCamera _camera;

        public CinemachineCamera CinemachineCamera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        private void Awake()
        {
            Instance = this;
            SceneEventHandler.SceneStarted += AddStackCamera;
        }

        void OnDestroy()
        {
            SceneEventHandler.SceneStarted -= AddStackCamera;
        }

        /// <summary>
        /// 카메라가 따라다니는 타겟 변경
        /// </summary>
        /// <param name="transform"></param>
        public void ChangeTarget(Transform transform)
        {
            _camera.Follow = transform;
        }

        private void AddStackCamera()
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_uiCamera);
        }
    }
}
