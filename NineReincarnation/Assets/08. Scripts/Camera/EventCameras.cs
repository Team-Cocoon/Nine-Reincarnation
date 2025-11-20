using Unity.Cinemachine;
using UnityEngine;

public class EventCameras : CinemachineExtension
{
    [SerializeField] private int _priority;
    [SerializeField] private int _endPriority;
    [SerializeField] private float _defaultOthoSize;
    [SerializeField] private CinemachineCamera _camera;
    
    public void Zoom()
    {

    }
    public void Shake()
    {

    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        
    }
}
