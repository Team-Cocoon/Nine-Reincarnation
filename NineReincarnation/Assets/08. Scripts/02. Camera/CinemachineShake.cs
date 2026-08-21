using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : CinemachineExtension
{
    private Vector3 _shakeOffset = Vector3.zero;

    private CancellationTokenSource _token;

    private float _currentStrength = 0f;

    private bool _isFinishRequested = false;

    public bool IsFinishRequested => _isFinishRequested;

    private void CancelCurrentShake()
    {
        if (_token != null)
        {
            _token.Cancel();
            _token.Dispose();
            _token = null;
        }
    }

    private void OnDisable()
    {
        CancelCurrentShake();
    }

    public async UniTask Shake(float duration, float strength)
    {
        CancelCurrentShake();
        _isFinishRequested = false;

        _token = new CancellationTokenSource();

        await ProcessShake(duration, strength, _token);
    }

    public async UniTask ShakeFadeOut(float fadeDuration = 1.0f)
    {
        // 현재 흔들리는 게 없으면 무시
        if (_currentStrength <= 0f) return;

        CancelCurrentShake();

        _token = new CancellationTokenSource();

        await ProcessFadeOut(fadeDuration, _currentStrength, _token);
    }

    public void StopShakeImmediate()
    {
        if (_currentStrength <= 0f) return;

        CancelCurrentShake();

        _shakeOffset = Vector3.zero;
        _currentStrength = 0f;
    }


    private async UniTaskVoid ShakeInfinite(CancellationTokenSource token)
    {
        while (!_token.IsCancellationRequested)
        {
            _shakeOffset = Random.insideUnitSphere * _currentStrength;

            await UniTask.NextFrame(token.Token);
        }
    }

    private async UniTask ProcessShake(float duration, float strength, CancellationTokenSource token)
    {
        _currentStrength = strength;
        float timer = 0f;

        if (duration == -1f)
        {
            _isFinishRequested = true;

            ShakeInfinite(token).Forget();
        }
        else
        {
            float shakeTime = duration;
            float fadeTime = 0.5f;

            while (timer < shakeTime)
            {
                timer += Time.deltaTime;
                _shakeOffset = Random.insideUnitSphere * _currentStrength;
                await UniTask.NextFrame(token.Token);
            }

            float startStrength = _currentStrength;

            await ProcessFadeOut(fadeTime, startStrength, _token);
        }
    }

    private async UniTask ProcessFadeOut(float duration, float startStrength, CancellationTokenSource token)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            _currentStrength = Mathf.Lerp(startStrength, 0f, t);
            _shakeOffset = Random.insideUnitSphere * _currentStrength;

            await UniTask.NextFrame(token.Token);
        }

        _shakeOffset = Vector3.zero;
        _currentStrength = 0f;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            state.RawPosition += _shakeOffset;
        }
    }
}
