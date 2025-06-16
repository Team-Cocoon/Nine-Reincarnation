using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Effect.WipeFade
{
    public class WipeFadeEffect
    {
        /// <summary>
        /// 서서히 밝아짐 (매개변수는 페이드 전용 머티리얼)
        /// </summary>
        public static void FadeIn(Material material, float duration, float delay = 0.0f, Action action = null)
        {
            float progress = 0f;
            material.SetFloat("_IsFadeIn", 1f);

            DOTween.To(() => progress, x =>
            {
                progress = x;
                material.SetFloat("_Progress", progress);
            }, 1.0f, duration).SetEase(Ease.Linear).SetUpdate(true).SetDelay(delay).OnComplete( () => { action?.Invoke();  });
        }

        /// <summary>
        /// 서서히 밝아짐 (매개변수는 그래픽)
        /// </summary>
        public static void FadeIn(Graphic graphic, float duration)
        {
            graphic.DOFade(1.0f, duration).SetEase(Ease.Linear).SetUpdate(true);
        }

        /// <summary>
        /// 서서히 어두워짐 (매개변수는 페이드 전용 머티리얼)
        /// </summary>
        public static void FadeOut(Material material, float duration)
        {
            float progress = 0f;
            material.SetFloat("_IsFadeIn", 0f);

            DOTween.To(() => progress, x =>
            {
                progress = x;
                material.SetFloat("_Progress", progress);
            }, 1.0f, duration).SetEase(Ease.Linear).SetUpdate(true);
        }

        /// <summary>
        /// 서서히 어두워짐 (매개변수는 그래픽)
        /// </summary>
        public static void FadeOut(Graphic graphic, float duration)
        {
            graphic.DOFade(0.0f, duration).SetEase(Ease.Linear).SetUpdate(true);
        }
    }
}