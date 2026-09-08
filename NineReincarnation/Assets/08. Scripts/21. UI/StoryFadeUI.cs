using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryFadeUI : MonoBehaviour
{
    [SerializeField] private InputConnector _inputConnector;
    [SerializeField] private Material _material;

    [Header("Setting")]
    [SerializeField] private bool _stopInputWhenFadeOut = false;

    private Material _runtimeMaterial;
    private Tween _fadeTween;

    private void Awake()
    {
        // Each story owns its fade state; never animate the shared material asset.
        _runtimeMaterial = Instantiate(_material);
        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            if (image.material == _material)
                image.material = _runtimeMaterial;
        }

        // The serialized material starts opaque. Keep the view clear until a fade event.
        FadeIn();
    }

    public async UniTask FadeInOut(float duration, float stayTime)
    {
        if (_stopInputWhenFadeOut && _inputConnector != null)
        {
            _inputConnector.InputManager?.ChangeActionToUI();
        }

        await FadeOut(duration);
        await UniTask.Delay(TimeSpan.FromSeconds(stayTime));
        await FadeIn(duration);

        if (_stopInputWhenFadeOut && _inputConnector != null)
        {
            _inputConnector.InputManager?.ChangeActionToPlayer();
        }
    }

    public async UniTask FadeOut(float duration)
    {
        _fadeTween?.Kill();
        _fadeTween = Effect.WipeFade.FadeEffect.WipeFadeOut(_runtimeMaterial, duration, true);
        await _fadeTween.AsyncWaitForCompletion();
    }
    public void FadeOut()
    {
        _fadeTween?.Kill();
        _runtimeMaterial.SetFloat("_isRight", 1f);
        _runtimeMaterial.SetFloat("_IsFadeIn", 0f);
        _runtimeMaterial.SetFloat("_Progress", 1f);
    }

    public async UniTask FadeIn(float duration)
    {
        _fadeTween?.Kill();
        _fadeTween = Effect.WipeFade.FadeEffect.WipeFadeIn(_runtimeMaterial, duration, true);
        await _fadeTween.AsyncWaitForCompletion();
    }

    public void FadeIn()
    {
        _fadeTween?.Kill();
        _runtimeMaterial.SetFloat("_isRight", 1f);
        _runtimeMaterial.SetFloat("_IsFadeIn", 1f);
        _runtimeMaterial.SetFloat("_Progress", 1f);
    }

    private void OnDestroy()
    {
        _fadeTween?.Kill();
        if (_runtimeMaterial != null)
            Destroy(_runtimeMaterial);
    }
}
