using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

namespace Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private Camera _uiCamera;

        [Inject]
        public void Constrct(Camera uiCamera)
        {
            _uiCamera = uiCamera;
            AddStackCamera(_uiCamera);
        }

        /// <summary>
        /// 카메라가 따라다니는 타겟 변경
        /// </summary>
        /// <param name="transform"></param>
        //public void ChangeTarget(Transform transform)
        //{
        //    if (_camera == null) return;
        //    _camera.Follow = transform;
        //}

        private void AddStackCamera(Camera camera)
        {
            _camera.GetUniversalAdditionalCameraData().cameraStack.Add(camera);
        }
    }
}
