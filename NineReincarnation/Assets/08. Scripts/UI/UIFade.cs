using Effect.WipeFade;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [Header("--- 페이드에 필요한 변수 ---")]
    [SerializeField] private Image _image;
    [SerializeField] private float _duration;

    private void OnEnable()
    {
        _image.material.SetFloat("_Progress", 0.0f);
        FadeEffect.WipeFadeIn(_image.material, _duration, true, 2.0f, () => { gameObject.SetActive(false); });
    }
}
