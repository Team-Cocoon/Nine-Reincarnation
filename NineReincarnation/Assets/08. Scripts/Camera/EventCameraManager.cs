using UnityEngine;

public class EventCameraManager : MonoBehaviour
{
    [SerializeField] private float _defaultOthoSize = 8.4375f;
    [SerializeField] private CinemachineShake _shake;
    [SerializeField] private CinemachineZoom _zoom;

    private void Start()
    {
        //_zoom.Zoom();
    }
}
