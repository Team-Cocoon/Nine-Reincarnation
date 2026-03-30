using DG.Tweening;
using Effect.WipeFade;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class FadeUI : GameUI
{
    [Header("--- 페이드에 필요한 변수 ---")]
    [SerializeField] private Image _image;
    [SerializeField] private float _duration;

    [Inject]
    private void Construct()
    {
        Material instancedMat = Instantiate(_image.material);

        _image.material = instancedMat;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public  Tween UIEvent_WipeFadeIn(bool stopTime)
    {
        _image.material.SetFloat("_Progress", 0.0f);


        return FadeEffect.WipeFadeIn(_image.material, _duration, false, 2.0f, () =>
        {
            ToggleUI();
        });
    }

    public Tween UIEvent_WipeFadeOut(bool stopTime)
    {
        _image.material.SetFloat("_Progress", 1.0f);

        ToggleUI();

        return FadeEffect.WipeFadeOut(_image.material, _duration, false, 0.0f);
    }

    public  Tween UIEvent_FadeIn()
    {
        OpenUI();
        _image.material.SetFloat("_Progress", 0.0f);
        _image.material.SetFloat("_IsFadeIn", 1.0f);
        Color color = _image.color;

        color.a = 1.0f;

        return FadeEffect.FadeIn(_image, 2.0f, () =>
        {
            CloseUI();
        });
    }

    public Tween UIEvent_FadeOut()
    {
        _image.material.SetFloat("_Progress", 1.0f);
        _image.material.SetFloat("_IsFadeIn", 0.0f);
        Color color = _image.color;
        color.a = 0.0f;
        _image.color = color;

        OpenUI();

        return FadeEffect.FadeOut(_image, 2.0f);
    }
}
