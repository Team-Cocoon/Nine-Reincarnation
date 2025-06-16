using System;
using DG.Tweening;
using Effect.WipeFade;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    [Header("--- 페이드에 필요한 변수 ---")]
    [SerializeField] private GameObject _fade;
    [SerializeField] private Image _image;
    [SerializeField] private float _duration;
    [SerializeField] private bool isStart;

    private void Awake()
    {
        UIEventHandler.OnSceneWipeFadeIn += WipeFadeIn;

        UIEventHandler.OnSceneFadeIn += FadeOut;

        UIEventHandler.OnSceneWipeFadeOut += WipeFadeOut;

        UIEventHandler.OnSceneFadeOut += FadeOut;

    }

    private void Start()
    {
        if (isStart)
        {
            WipeFadeIn();
        }
    }

    private void OnDestroy()
    {
        UIEventHandler.OnSceneWipeFadeIn -= WipeFadeIn;

        UIEventHandler.OnSceneFadeIn -= FadeOut;

        UIEventHandler.OnSceneWipeFadeOut -= WipeFadeOut;

        UIEventHandler.OnSceneFadeOut -= FadeOut;
    }

    private void WipeFadeIn(Action action = null)
    {
        _fade.SetActive(true);
        _image.material.SetFloat("_Progress", 0.0f);
        FadeEffect.WipeFadeIn(_image.material, _duration, false, 2.0f, () =>
        {
            _fade.SetActive(false);
            action?.Invoke();
        });
    }

    private void WipeFadeOut(Action action = null)
    {
        _fade.SetActive(true);
        _image.material.SetFloat("_Progress", 1.0f);
        FadeEffect.WipeFadeOut(_image.material, _duration, false, 0.0f, () => { 
            action?.Invoke();
        });
    }

    private void FadeIn(Action action = null)
    {
        _fade.SetActive(true);
        FadeEffect.FadeIn(_image, 2.0f, action);
    }

    private void FadeOut(Action action = null)
    {
        _fade.SetActive(true);
        _image.material.SetFloat("_Progress", 0.0f);
        Color color = _image.color;
        color.a = 0.0f;

        _image.color = color;
        FadeEffect.FadeOut(_image, 2.0f, action);
    }
}
