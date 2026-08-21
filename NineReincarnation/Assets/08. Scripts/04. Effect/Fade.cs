using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField] private Image _image;
    [Header("페이드 시간")]
    [SerializeField] private float _fadeTime = 1f;

    private float _time = 0f;

    public void FadeInStart(Action fadeEnd = null)
    {
        StartCoroutine(FadeIn(fadeEnd));
    }
    private IEnumerator FadeIn(Action fadeEnd)
    {
        _image.gameObject.SetActive(true);
        Color alpha = _image.color;
        while (alpha.a > 0f)
        {
            _time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(1, 0, _time);
            _image.color = alpha;
            yield return null;
        }
        _image.gameObject.SetActive(false);
        fadeEnd?.Invoke();
        yield return null;
    }
}
