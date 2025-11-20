using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : CinemachineExtension
{
    private Vector3 shakeOffset = Vector3.zero;
    private Tween _tween;
    private float _strength;

    /// <summary>
    /// 카메라 흔들기
    /// </summary>
    public async UniTask Shake(float duration, float strength)
    {
        // 기존 셰이크 값 초기화
        shakeOffset = Vector3.zero;

        await DOTween.Shake(
            () => shakeOffset,
            x => shakeOffset = x,
            duration,
            strength
        ).ToUniTask(cancellationToken: this.destroyCancellationToken);
    }

    public void UpdateShake(float newStrength)
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();

        _tween = DOTween.Shake(
            () => shakeOffset,
            x => shakeOffset = x,
            0.5f,          // 갱신용 짧은 duration
            newStrength
        );
    }

    public void StopShake()
    { 
    
    }




    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Follow, Aim 모두 계산한 최종 단계에서 위치를 덮어쓰기
        if (stage == CinemachineCore.Stage.Finalize)
        {
            state.RawPosition += shakeOffset;
        }
    }
}
