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
        var tween = Effect.WipeFade.FadeEffect.WipeFadeOut(_material, duration, true);
        await tween.AsyncWaitForCompletion();
    }

    public async UniTask FadeIn(float duration)
    {
        var tween = Effect.WipeFade.FadeEffect.WipeFadeIn(_material, duration, true);
        await tween.AsyncWaitForCompletion();
    }
}
