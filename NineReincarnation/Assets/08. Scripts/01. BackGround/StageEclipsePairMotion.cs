using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public sealed class StageEclipsePairMotion : MonoBehaviour
{
    [Header("Pair")]
    [SerializeField] private Transform sun;
    [SerializeField] private Transform moon;

    [Header("Loop Motion")]
    [SerializeField] private Vector2 sunAxis = new(-1f, 0.25f);
    [SerializeField] private Vector2 moonAxis = new(1f, -0.25f);
    [SerializeField, Min(0f)] private float sunAmplitude = 0.08f;
    [SerializeField, Min(0f)] private float moonAmplitude = 0.1f;
    [Tooltip("한 방향으로 이동하는 시간입니다. 왕복 전체 주기는 이 값의 두 배입니다.")]
    [SerializeField, Min(0.1f)] private float oneWayDuration = 1.5f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Vector3 initialSunLocalPosition;
    private Vector3 initialMoonLocalPosition;
    private bool positionsCaptured;
    private Tween sunTween;
    private Tween moonTween;

    private void OnEnable()
    {
        CaptureInitialPositions();

        if (Application.isPlaying)
        {
            StartMotion();
        }
    }

    private void OnDisable()
    {
        StopMotion();

        if (!positionsCaptured)
        {
            return;
        }

        if (sun != null)
        {
            sun.localPosition = initialSunLocalPosition;
        }

        if (moon != null)
        {
            moon.localPosition = initialMoonLocalPosition;
        }
    }

    private void OnValidate()
    {
        sunAmplitude = Mathf.Max(0f, sunAmplitude);
        moonAmplitude = Mathf.Max(0f, moonAmplitude);
        oneWayDuration = Mathf.Max(0.1f, oneWayDuration);
        positionsCaptured = false;
    }

    private void StartMotion()
    {
        if (!positionsCaptured)
        {
            CaptureInitialPositions();
        }

        if (!positionsCaptured)
        {
            return;
        }

        StopMotion();

        Vector3 sunTarget = initialSunLocalPosition +
            (Vector3)(GetDirection(sunAxis) * sunAmplitude);
        Vector3 moonTarget = initialMoonLocalPosition +
            (Vector3)(GetDirection(moonAxis) * moonAmplitude);

        sunTween = sun.DOLocalMove(sunTarget, oneWayDuration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);

        moonTween = moon.DOLocalMove(moonTarget, oneWayDuration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopMotion()
    {
        sunTween?.Kill(false);
        moonTween?.Kill(false);
        sunTween = null;
        moonTween = null;
    }

    private void CaptureInitialPositions()
    {
        sun ??= transform.Find("8sun");
        moon ??= transform.Find("9moon");

        if (sun == null || moon == null)
        {
            positionsCaptured = false;
            return;
        }

        initialSunLocalPosition = sun.localPosition;
        initialMoonLocalPosition = moon.localPosition;
        positionsCaptured = true;
    }

    private static Vector2 GetDirection(Vector2 axis)
    {
        return axis.sqrMagnitude > Mathf.Epsilon ? axis.normalized : Vector2.zero;
    }
}
