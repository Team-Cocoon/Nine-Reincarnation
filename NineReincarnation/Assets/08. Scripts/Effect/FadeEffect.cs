using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Effect.WipeFade
{
    public class FadeEffect
    {
        /// <summary>
        /// 서서히 밝아짐 (매개변수는 페이드 전용 머티리얼)
        /// </summary>
        public static Tween WipeFadeIn(Material material, float duration, bool isRight, float delay = 0.0f, Action action = null)
        {
            float progress = 0f;
            material.SetFloat("_isRight", isRight == true ? 1f : 0f);
            material.SetFloat("_IsFadeIn", 1f);

            return DOTween.To(() => progress, x =>
            {
                progress = x;
                material.SetFloat("_Progress", progress);
            }, 1.0f, duration).SetEase(Ease.Linear).SetUpdate(true).SetDelay(delay).OnComplete(() => { action?.Invoke(); });
        }

        /// <summary>
        /// 서서히 밝아짐 (매개변수는 그래픽)
        /// </summary>
        public static Tween FadeIn(Graphic graphic, float duration, Action action = null)
        {
            return graphic.DOFade(0.0f, duration).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => { action?.Invoke(); });
        }

        /// <summary>
        /// 서서히 어두워짐 (매개변수는 페이드 전용 머티리얼)
        /// </summary>
        public static Tween WipeFadeOut(Material material, float duration, bool isRight, float delay = 0.0f, Action action = null)
        {
            float progress = 0f;
            material.SetFloat("_isRight", isRight == true ? 1f : 0f);
            material.SetFloat("_IsFadeIn", 0f);

            Sequence seq = DOTween.Sequence();

            return seq.Join(DOTween.To(() => progress, x =>
            {
                progress = x;
                material.SetFloat("_Progress", progress);
            }, 1.0f, duration).SetEase(Ease.Linear))
            .AppendInterval(1.0f)
            .SetDelay(delay).SetUpdate(true).OnComplete(() => { DOTween.KillAll(); action?.Invoke(); });
        }

        /// <summary>
        /// 서서히 어두워짐 (매개변수는 그래픽)
        /// </summary>
        public static Tween FadeOut(Graphic graphic, float duration, Action action = null)
        {
            return graphic.DOFade(1.0f, duration).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => { DOTween.KillAll(); action?.Invoke(); });
        }
    }
}