using UnityEngine;

public class EventCameraManager : MonoBehaviour
{
    [SerializeField] private float _defaultOthoSize = 8.4375f;
    [SerializeField] private CinemachineShake _shake;
    [SerializeField] private CinemachineZoom _zoom;

    private async void Start()
    {
        await _zoom.Zoom(3.5f, 0.8f);
    }
}
