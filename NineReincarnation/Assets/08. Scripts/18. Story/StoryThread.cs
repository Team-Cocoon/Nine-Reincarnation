using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryThread : Thread
{
    const float DefaultFadeTime = 1f;

    [Header("DialogueThread")]
    [SerializeField] private float _fadeTime = 1f;

    private bool _isConnected = false;
    private bool _isFading = false;

    protected override void Start()
    {
        base.Start();

        if (_lineRenderer != null)
            _lineRenderer.enabled = false;
    }

    protected override void UpdateThread()
    {
        if (!_isConnected) return;
        if (_isFading) return;
        if (_startTransform == null || _endTransform == null) return;

        UpdateSegments();
    }

    public void Connect(Transform start, Transform end, float fadeTime = DefaultFadeTime)
    {
        _currentJobHandle.Complete();

        _startTransform = start;
        _endTransform = end;

        _fadeTime = fadeTime;

        ResetSegments();

        _isConnected = true;
        _isFading = false;

        if (_lineRenderer != null)
        {
            ResetAlpha();
            _lineRenderer.enabled = true;
        }
    }

    public void Disconnect()
    {
        if (!_isConnected) return;
        if (_isFading) return;

        FadeOutAsync().Forget();
    }

    private async UniTaskVoid FadeOutAsync()
    {
        _isFading = true;

        _currentJobHandle.Complete();

        if (_lineRenderer == null)
        {
            _isConnected = false;
            _isFading = false;
            return;
        }

        Color startColor = _lineRenderer.startColor;
        Color endColor = _lineRenderer.endColor;

        Color targetStartColor = startColor;
        Color targetEndColor = endColor;

        targetStartColor.a = 0f;
        targetEndColor.a = 0f;

        float elapsed = 0f;

        while (elapsed < _fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _fadeTime);

            _lineRenderer.startColor = Color.Lerp(startColor, targetStartColor, t);
            _lineRenderer.endColor = Color.Lerp(endColor, targetEndColor, t);

            if (_startTransform != null && _endTransform != null)
            {
                _currentJobHandle.Complete();
                UpdateSegments();
                _currentJobHandle.Complete();
                RenderThread();
            }

            await UniTask.Yield();
        }

        _currentJobHandle.Complete();

        _lineRenderer.enabled = false;
        ResetAlpha();

        _isConnected = false;
        _isFading = false;

        _startTransform = null;
        _endTransform = null;
    }

    private void ResetAlpha()
    {
        if (_lineRenderer == null) return;

        Color startColor = _lineRenderer.startColor;
        Color endColor = _lineRenderer.endColor;

        startColor.a = 1f;
        endColor.a = 1f;

        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

    private void ResetSegments()
    {
        if (_startTransform == null || _endTransform == null) return;
        if (!segments.IsCreated) return;

        Vector2 start = _startTransform.position;
        Vector2 end = _endTransform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector2 pos = Vector2.Lerp(start, end, t);

            segments[i] = new Segment(pos);
        }

        RenderThread();
    }
}
