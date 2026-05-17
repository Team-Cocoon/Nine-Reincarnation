using UnityEngine;
using System;
using System.Collections;

public class ThrowRedThread : ThrowThread
{
    [Header("연결 후 실제 라인 설정")]
    [SerializeField] private GameObject linkedLineObject; 
    [SerializeField] private LineRenderer linkedLineRenderer;
    
    // 추가: 실이 연결된 후 유지할 수 있는 최대 거리 (예: 10)
    [SerializeField] private float _maintainDistance = 10f; 

    public event Action<float> OnDistanceUpdate;
    private bool _isVisualSwitched = false;

    protected override void UpdateThreadVisualization()
    {
        if (!_isVisualSwitched)
        {
            UpdateSegments();
        }
        else
        {
            linkedLineRenderer.SetPosition(0, _startTransform.position);
            linkedLineRenderer.SetPosition(1, targetTransform.position);
        }
    }

    protected override void ThrowingEvent()
    {
        StartCoroutine(WaitAndTransfer());
    }

    protected override void StartDeleting()
    {
        base.StartDeleting();

        StartCoroutine(DisappearingCustom(false, () => {
            _state = ThrowThreadState.Idle;
            _isVisualSwitched = false;
        }));
    }

    private IEnumerator WaitAndTransfer()
    {
        yield return new WaitForSeconds(0.1f); 

        StartCoroutine(DisappearingCustom(true, () => {
            _isVisualSwitched = true;
        }));
    }

    private IEnumerator DisappearingCustom(bool isSwitching, System.Action onComplete)
    {
        if (_lineRenderer == null) yield break;

        // isSwitching이 false라는 것은 실이 완전히 끊어지며 삭제됨을 의미함
        // 이때 Feather 측의 EnableClickInteraction()을 호출하여 상호작용 상태를 해제함
        if (!isSwitching)
        {
            clickable?.EnableClickInteraction();
            clickable = null;
        }
        else if (linkedLineRenderer != null)
        {
            linkedLineRenderer.enabled = true;
        }

        float elapsedTime = 0f;
        float duration = 1f;

        Color startColorOrig = _lineRenderer.startColor;
        Color endColorOrig = new Color(startColorOrig.r, startColorOrig.g, startColorOrig.b, 0f);

        Color startColorLinked = Color.white;
        if (linkedLineRenderer != null) startColorLinked = linkedLineRenderer.startColor;
        Color endColorLinked = new Color(startColorLinked.r, startColorLinked.g, startColorLinked.b, 0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            Color currentColor = Color.Lerp(startColorOrig, endColorOrig, normalizedTime);

            if (_lineRenderer.enabled)
            {
                _lineRenderer.startColor = currentColor;
                _lineRenderer.endColor = currentColor;
            }

            if (targetTransform != null && linkedLineRenderer != null && linkedLineRenderer.enabled)
            {
                linkedLineRenderer.SetPosition(0, _startTransform.position);
                linkedLineRenderer.SetPosition(1, targetTransform.position);

                if (!isSwitching)
                {
                    Color currentLinked = Color.Lerp(startColorLinked, endColorLinked, normalizedTime);
                    linkedLineRenderer.startColor = currentLinked;
                    linkedLineRenderer.endColor = currentLinked;
                }
            }

            yield return null;
        }

        if (_lineRenderer != null)
        {
            if (isSwitching)
            {
                _lineRenderer.enabled = false;
                ResetAlpha();
            }
            else
            {
                if (linkedLineRenderer != null) linkedLineRenderer.enabled = false;
                _lineRenderer.enabled = false;
                InitThread();
                ResetAlpha();
            }
        }

        onComplete?.Invoke();
    }

    protected override void ResetAlpha()
    {
        base.ResetAlpha();

        if (linkedLineRenderer != null)
        {
            Color c = linkedLineRenderer.startColor;
            c.a = 1f;
            linkedLineRenderer.startColor = c;
            linkedLineRenderer.endColor = c;
        }
    }

    // 핵심 로직: 상태에 따라 유효 거리를 다르게 판단
    public override bool IsExpired()
    {
        targetPos = targetTransform.position;
        float currentDistance = Vector3.Distance(_startTransform.position, targetPos);

        // 연결되기 전(던지는 중)이면 기존 limitDistance, 연결 후면 _maintainDistance 사용
        float currentLimitDistance = _isVisualSwitched ? _maintainDistance : limitDistance;

        float ratio = Mathf.Clamp01(currentDistance / currentLimitDistance);
        OnDistanceUpdate?.Invoke(ratio);

        // 현재 한계 거리를 벗어나면 만료(끊어짐) 처리
        if (currentDistance > currentLimitDistance)
        {
            return true; 
        }
        return false;
    }
}