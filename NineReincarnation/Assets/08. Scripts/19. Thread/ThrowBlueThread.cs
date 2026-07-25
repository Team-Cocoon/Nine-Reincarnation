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
        UIEventHandler.OnBlueThreadConnectionChanged_Invoke(true);
        //_connectionStartTime = Time.time;
        Debug.Log(_player.BlueThread);
    }

    protected override void StartDeleting()
    {
        base.StartDeleting();
        StartCoroutine(Disappearing(() => { _state = ThrowThreadState.Idle; }));
    }

    // 전환 시 이전에 연결됐던 오브젝트를 즉시 연결 전 상태로 되돌린다.
    protected override void ReleaseTarget()
    {
        ForceDisconnectTarget();
        clickable = null;
        UIEventHandler.OnBlueThreadConnectionChanged_Invoke(false);
    }

    // 사용자가 직접 연결을 취소한 경우에도 대상 오브젝트를 즉시 연결 전 상태로 되돌린다.
    // (자연 만료 시에는 오브젝트가 스스로 부드럽게 복귀하므로 이 훅이 호출되지 않는다.)
    protected override void OnManualCancel()
    {
        ForceDisconnectTarget();
    }

    // 현재 연결된 청연 대상을 즉시 연결 전 상태(불투명/충돌 복구)로 되돌린다.
    private void ForceDisconnectTarget()
    {
        if (targetTransform != null)
        {
            var phasable = targetTransform.GetComponent<IPhasable>();
            phasable?.ForceDisconnect();
        }
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
        UIEventHandler.OnBlueThreadConnectionChanged_Invoke(false);
        onComplete?.Invoke();
    }

    public override bool IsExpired()
    {
        //if (_connectionStartTime < 0) return false;
        //return Time.time >= (_connectionStartTime + _lifeTime);
        return (InputManager.Instance.CurPlayer.ActivePhasingCount == 0);
    }
}
