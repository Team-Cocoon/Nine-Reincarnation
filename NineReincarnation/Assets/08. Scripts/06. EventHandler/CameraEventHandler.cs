using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace EventHandler
{
    public static class CameraEventHandler
    {
        /// <summary>
        /// 플레이어 카메라 Look 이벤트
        /// </summary>
        public static event Action<bool> OnLook;
        public static void OnLook_Invoke(bool isTrue) => OnLook?.Invoke(isTrue);

        /// <summary>
        /// 카메라 셰이크
        /// </summary>
        /// <param name="duration"> 시간 </param>
        /// <param name="strength"> 강도 </param>
        /// <param name="vibrato"> 진동 횟수 </param>
        /// <param name="randomness"> 무작위성 </param>
        /// <param name="isFadeOut"> 시간이 지날수록 흔들림을 줄일 것인지 </param>
        public static void Shake(Camera camera, float duration, float strength, int vibrato, float randomness, bool isFadeOut, TweenCallback callbackAction = null)
        {
            Tween tween = camera.DOShakePosition(duration, strength, vibrato, randomness, isFadeOut);
            if (callbackAction != null)
            {
                tween.OnComplete(callbackAction);
            }
        }

        /// <summary>
        /// 카메라 줌 기능
        /// </summary>
        /// <param name="endValue"> 최종 값 </param>
        /// <param name="duration"> 시간 </param>
        public static void Zoom(Camera camera, float endValue, float duration, TweenCallback callbackAction = null)
        {
            Tween tween = camera.DOOrthoSize(endValue, duration);
            if (callbackAction != null)
            {
                tween.OnComplete(callbackAction);
            }
        }

        /// <summary>
        /// 카메라 위치는 고정한 채, 메인 카메라와 시네머신 카메라의 줌만 조절합니다.
        /// </summary>
        /// <param name="camera">메인 카메라</param>
        /// <param name="cinemachineCamera">타겟 시네머신 가상 카메라</param>
        /// <param name="endValue">목표 Orthographic Size</param>
        /// <param name="duration">애니메이션 시간</param>
        /// <param name="callbackAction">완료 시 실행될 콜백</param>
        /// <returns>생성된 DOTween 시퀀스</returns>
        public static void Zoom(Camera camera, CinemachineCamera cinemachineCamera, float endValue, float duration, TweenCallback callbackAction = null)
        {
            Sequence zoomSequence = DOTween.Sequence();

            // 1. 메인 카메라의 OrthoSize 변경
            zoomSequence.Join(camera.DOOrthoSize(endValue, duration));

            // 2. Cinemachine 카메라의 OrthographicSize 변경
            zoomSequence.Join(DOTween.To(
                () => cinemachineCamera.Lens.OrthographicSize,
                x =>
                {
                    var lens = cinemachineCamera.Lens;
                    lens.OrthographicSize = x;
                    cinemachineCamera.Lens = lens;
                },
                endValue,
                duration));

            if (callbackAction != null)
            {
                zoomSequence.OnComplete(callbackAction);
            }
        }

        /// <summary>
        /// 카메라를 특정 위치를 중심으로 줌합니다.
        /// </summary>
        /// <param name="camera">대상 카메라</param>
        /// <param name="targetPosition">줌의 중심이 될 월드 좌표</param>
        /// <param name="endValue">목표 Orthographic Size</param>
        /// <param name="duration">애니메이션 시간</param>
        /// <param name="callbackAction">완료 시 실행될 콜백 함수</param>
        public static void ZoomToTarget(Camera camera, Vector3 targetPosition, float endValue, float duration, TweenCallback callbackAction = null)
        {
            Sequence zoomSequence = DOTween.Sequence();

            Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, camera.transform.position.z);
            zoomSequence.Join(camera.transform.DOMove(endPosition, duration));
            zoomSequence.Join(camera.DOOrthoSize(endValue, duration));

            if (callbackAction != null)
            {
                zoomSequence.OnComplete(callbackAction);
            }
        }

        public static void ZoomToTarget(Camera camera, CinemachineCamera cinemachineCamera, Vector3 targetPosition, float endValue, float duration, TweenCallback callbackAction = null)
        {
            Sequence zoomSequence = DOTween.Sequence();

            // 1. 메인 카메라 위치 이동 (기존과 동일)
            Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, camera.transform.position.z);
            zoomSequence.Join(camera.transform.DOMove(endPosition, duration));

            // 2. 메인 카메라 OrthoSize 변경 (기존과 동일)
            zoomSequence.Join(camera.DOOrthoSize(endValue, duration));

            // 3. ✨ Cinemachine 카메라의 OrthographicSize를 DOTween.To()로 변경
            // getter: 현재 렌즈 크기를 가져옴
            // setter: 매 프레임마다 계산된 새 값(x)으로 렌즈 설정을 통째로 교체
            zoomSequence.Join(DOTween.To(
                () => cinemachineCamera.Lens.OrthographicSize,
                x =>
                {
                    var lens = cinemachineCamera.Lens; // 현재 렌즈 설정 '복사'
                    lens.OrthographicSize = x;           // 복사본의 값 변경
                    cinemachineCamera.Lens = lens;     // 변경된 복사본을 다시 '할당'
                },
                endValue,
                duration));

            if (callbackAction != null)
            {
                zoomSequence.OnComplete(callbackAction);
            }
        }

        /// <summary>
        /// [Sequence] 현재 위치에서 줌인했다가, 잠시 후 지정된 사이즈로 돌아옵니다.
        /// </summary>
        public static Sequence ZoomInOut(Camera camera, CinemachineCamera cinemachineCamera, float zoomInValue, float returnOrthoSize, float duration, float delay = 0f, TweenCallback callbackAction = null)
        {
            Sequence mainSequence = DOTween.Sequence();

            // Append 안에 직접 트윈들을 생성하여 추가합니다.
            mainSequence.Append(camera.DOOrthoSize(zoomInValue, duration));
            mainSequence.Join(DOTween.To(() => cinemachineCamera.Lens.OrthographicSize, x => { var lens = cinemachineCamera.Lens; lens.OrthographicSize = x; cinemachineCamera.Lens = lens; }, zoomInValue, duration));

            mainSequence.AppendInterval(delay);

            mainSequence.Append(camera.DOOrthoSize(returnOrthoSize, duration));
            mainSequence.Join(DOTween.To(() => cinemachineCamera.Lens.OrthographicSize, x => { var lens = cinemachineCamera.Lens; lens.OrthographicSize = x; cinemachineCamera.Lens = lens; }, returnOrthoSize, duration));

            mainSequence.OnComplete(callbackAction);
            return mainSequence;
        }
    }

}
