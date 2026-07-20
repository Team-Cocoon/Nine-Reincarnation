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
    [SerializeField] private VirtualCameraManager _cameraManager;
    [SerializeField] private StoryFadeUI _fadeUI;
    [SerializeField] private bool _playSound = false;
    [SerializeField] private float _defaultCameraShiftDampingTime = 0.5f;
    private bool _hasSkipEvent = false;
    public bool HasSkipEvent => _hasSkipEvent;

    public void ZoomIn()
    {
        Zoom(1.0f, 7.0f).Forget();
    }

    public async UniTask ExcuteEvent(CameraClass data)
    {
        _hasSkipEvent = false;

        string name = data.Name;
        CameraEventType type = data.Type;
        float duration = data.Duration;
        float size = data.Size;

        switch (type)
        {
            case CameraEventType.Shake:
                if (duration == -1.0f)
                {
                    _hasSkipEvent = true;
                }

                if (_playSound)
                {
                    AudioManager.Instance?.PlaySfx(AudioManager.Sfx.CameraShake);
                }
                await Shake(duration, size);

                break;
            case CameraEventType.ZoomIn:
            case CameraEventType.ZoomOut:
                await Zoom(duration, size);
                break;
            case CameraEventType.CameraShift:
                await CameraShift(name);
                break;
            case CameraEventType.FadeIn:
                await _fadeUI.FadeIn(duration);
                break;
            case CameraEventType.FadeOut:
                await _fadeUI.FadeOut(duration);
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

    private async UniTask CameraShift(string name)
    {
        Debug.Log($"Shift to {name}");
        _cameraManager.SetFollowObj(name);
        await UniTask.WaitForSeconds(_defaultCameraShiftDampingTime);
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
        if (_shake.IsFinishRequested)
        {
            await _shake.ShakeFadeOut();
        }
    }

    public async UniTask ZoomDefault()
    {
        await _zoom.Zoom(1.0f, _defaultOthoSize);
    }
}
