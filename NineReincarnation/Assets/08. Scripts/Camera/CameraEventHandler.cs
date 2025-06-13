using System;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;

namespace EventHandler
{
    public class CameraEventHandler
    {
        /// <summary>
        /// 플레이어 카메라 Look 이벤트
        /// </summary>
        public static Action<bool> OnLook;

        /// <summary>
        /// 카메라 셰이크
        /// </summary>
        /// <param name="duration"> 시간 </param>
        /// <param name="strength"> 강도 </param>
        /// <param name="vibrato"> 진동 횟수 </param>
        /// <param name="randomness"> 무작위성 </param>
        /// <param name="isFadeOut"> 시간이 지날수록 흔들림을 줄일 것인지 </param>
        public static void Shake(Camera camera, float duration, float strength, int vibrato, float randomness, bool isFadeOut)
        {
            camera.DOShakePosition(duration, strength, vibrato, randomness, isFadeOut);
        }

        /// <summary>
        /// 카메라 줌 기능
        /// </summary>
        /// <param name="endValue"> 최종 값 </param>
        /// <param name="duration"> 시간 </param>
        public static void Zoom(Camera camera, float endValue, float duration)
        {
            camera.DOOrthoSize(endValue, duration);
        }
    }

}
