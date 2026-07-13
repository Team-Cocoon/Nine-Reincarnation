using System.Collections;
using UnityEngine;

public class ThrowBlueThread : ThrowThread
{
    //[SerializeField] private float _lifeTime = 0.5f; 
    //private float _connectionStartTime = -1f; 

    protected override bool CanThrow()
    {
        return _player.BlueThread > 0;
    }

    protected override void ThrowingEvent()
    {
        _player.BlueThread--;
        //_connectionStartTime = Time.time;
        Debug.Log(_player.BlueThread);
    }

    protected override void StartDeleting()
    {
        base.StartDeleting();
        StartCoroutine(Disappearing(() => { _state = ThrowThreadState.Idle; }));
    }

    private IEnumerator Disappearing(System.Action onComplete)
    {
        // 상호작용 종료
        // clickable?.EnableClickInteraction();
        clickable = null;

        float elapsedTime = 0f;
        Color startColor = _lineRenderer.startColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / 1f);

            _lineRenderer.startColor = currentColor;
            _lineRenderer.endColor = currentColor;

            yield return null;
        }
        _lineRenderer.startColor = endColor;
        _lineRenderer.endColor = endColor;

        InitThread();
        _lineRenderer.enabled = false;
        ResetAlpha();
        onComplete?.Invoke();
    }

    public override bool IsExpired()
    {
        //if (_connectionStartTime < 0) return false;
        //return Time.time >= (_connectionStartTime + _lifeTime);
        return (InputManager.Instance.CurPlayer.ActivePhasingCount == 0);
    }
}
