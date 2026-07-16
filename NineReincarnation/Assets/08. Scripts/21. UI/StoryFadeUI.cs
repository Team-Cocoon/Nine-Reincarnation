using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryFadeUI : MonoBehaviour
{
    [SerializeField] private InputConnector _inputConnector;
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private Material _material;

    [Header("Setting")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _fadeStayTime = 1f;
    [SerializeField] private bool _fadeInOutAfterDialogue = false;
    [SerializeField] private bool _stopInputWhenFadeOut = false;

    private void Awake()
    {
        if (_fadeInOutAfterDialogue)
        {
            _dialogueManager.DialogueEndAddListener(
                () => FadeInOut(_fadeDuration, _fadeStayTime).Forget()
            );
        }
    }

    private async UniTask FadeInOut(float duration, float stayTime)
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
