using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineZoom : CinemachineExtension
{
    private float sizeOffset = 0f;

    public async UniTask Zoom(float duration, float amount)
    {
        await DOTween.To(
            () => sizeOffset,
            x => sizeOffset = x,
            amount,
            duration
        ).ToUniTask(cancellationToken: this.destroyCancellationToken);
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
            state.Lens.OrthographicSize += sizeOffset;
        }
    }
}
