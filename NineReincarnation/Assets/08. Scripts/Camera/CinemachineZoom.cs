using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;

public class CinemachineZoom : CinemachineExtension
{
    private CinemachineCamera _cam;
    private float sizeOffset = 0f;

    protected override void Awake()
    {
        base.Awake();
        _cam = GetComponent<CinemachineCamera>();
    }

    public async UniTask Zoom(float duration, float amount)
    {
        sizeOffset = _cam.Lens.OrthographicSize;

        await DOTween.To(
            () => sizeOffset,
            x => sizeOffset = x,
            amount,
            duration
        ).SetEase(Ease.OutSine)
        .ToUniTask(cancellationToken: this.destroyCancellationToken);
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            state.Lens.OrthographicSize = sizeOffset;
        }
    }
}
