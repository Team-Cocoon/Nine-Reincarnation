using UnityEngine;
using System;
using System.Collections;

public class ThrowRedThread : ThrowThread
{
    [Header("연결 후 실제 라인 설정")]
    [SerializeField] private GameObject linkedLineObject; // 연결 후 보여줄 실제 줄 오브젝트
    [SerializeField] private LineRenderer linkedLineRenderer;

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

        // 초기 색상 안전하게 저장
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

    public override bool IsExpired()
    {
        targetPos = targetTransform.position;
        float currentDistance = Vector3.Distance(_startTransform.position, targetPos);

        float ratio = Mathf.Clamp01(currentDistance / limitDistance);
        OnDistanceUpdate?.Invoke(ratio);

        if (currentDistance > limitDistance)
        {
            return true;
        }
        return false;
    }
}
