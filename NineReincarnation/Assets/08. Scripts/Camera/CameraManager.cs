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

        private void Awake()
        {
            Instance = this;
            //_camera = GetComponent<CinemachineCamera>();
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
