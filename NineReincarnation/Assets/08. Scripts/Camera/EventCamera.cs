using System.Drawing;
using Cysharp.Threading.Tasks;
using ExcelData;
using UnityEngine;

public class EventCamera : MonoBehaviour
{
    [SerializeField] private int _priority;
    [SerializeField] private int _endPriority;
    [SerializeField] private float _defaultOthoSize;
    [SerializeField] private CinemachineZoom _zoom;
    [SerializeField] private CinemachineShake _shake;

    private bool _hasSkipEvent = false;
    public bool HasSkipEvent => _hasSkipEvent;


    public async UniTask ExcuteEvent(CameraClass data)
    {
        _hasSkipEvent = false;

        CameraEventType type = data.Type;
        float duration = data.Duration;
        float size = data.Size;

        switch (type)
        {
            case CameraEventType.Shake:
                if(duration == -1.0f)
                {
                    _hasSkipEvent = true;
                }
                await Shake(duration, size);
                break;
            case CameraEventType.ZoomIn:
            case CameraEventType.ZoomOut:
                await Zoom(duration, size);
                break;
        }
    }

    private async UniTask Zoom(float duration, float size)
    {
        await _zoom.Zoom(duration, size);
    }

    private async UniTask Shake(float duration, float strength)
    {
        await _shake.Shake(duration, strength);
    }

    public void StopShakeImmediate()
    {
        if (_shake.IsFinishRequested)
        {
            _shake.StopShakeImmediate();
        }
    }

    public async UniTask CancelShake()
    {
        if(_shake.IsFinishRequested)
        {
            await _shake.ShakeFadeOut();
        }
    }

    public async UniTask ZoomDefault()
    {
        await _zoom.Zoom(1.0f, _defaultOthoSize);
    }
}
