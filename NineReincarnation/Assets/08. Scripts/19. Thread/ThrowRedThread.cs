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

    public override void ResetThread()
    {
        base.ResetThread();
        _isVisualSwitched = false;
        if (linkedLineRenderer != null) linkedLineRenderer.enabled = false;
    }

    protected override void UpdateThreadVisualization()
    {
        if (!_isVisualSwitched)
        {
            UpdateSegments();
        }
        else
        {
            // [안전장치] 타겟이나 시작점이 소멸했을 때의 널 예외 방지
            if (_startTransform != null && targetTransform != null && linkedLineRenderer != null)
            {
                linkedLineRenderer.SetPosition(0, _startTransform.position);
                linkedLineRenderer.SetPosition(1, targetTransform.position);
            }
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

    // 전환(즉시 해제 후 재연결) 시에는 페이드 없이 연결선 비주얼을 즉시 정리한다.
    protected override void CancelConnection()
    {
        base.CancelConnection();
        _isVisualSwitched = false;
        if (linkedLineRenderer != null) linkedLineRenderer.enabled = false;
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

            if (_lineRenderer != null && _lineRenderer.enabled)
            {
                Color currentColor = Color.Lerp(startColorOrig, endColorOrig, normalizedTime);
                _lineRenderer.startColor = currentColor;
                _lineRenderer.endColor = currentColor;
            }

            // 페이드아웃 루프가 도는 도중 실시간 널 체크를 적용하여 낙하 연출 지원 및 에러 방지
            if (_startTransform != null && targetTransform != null && linkedLineRenderer != null && linkedLineRenderer.enabled)
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
        // [안전장치] 타겟 오브젝트나 플레이어가 사라진 상태라면 만료(true) 처리하여 안전하게 연결 종료
        if (targetTransform == null || _startTransform == null)
        {
            return true;
        }

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
