using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

public class CrackedPlatform : MonoBehaviour, ICollidable
{
    private enum PlatformState { Idle, Preparing, Falling }

    [Header("Settings")]
    [SerializeField] private float _waitTime = 0.5f;
    [SerializeField] private float _shakeTime = 0.2f;
    [SerializeField] private float _fallDistance = 15f;
    [SerializeField] private float _fallDuration = 1.5f;

    private PlatformState _currentState = PlatformState.Idle;
    private CancellationTokenSource _breakCts;
    private Vector3 _originPosition;
    private Collider2D _collider;

    private void Awake()
    {
        _originPosition = transform.position;
        _collider = GetComponent<Collider2D>();
    }

    public void Enter(GameObject go = null)
    {
        if (_currentState != PlatformState.Idle) return;

        _breakCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        BreakSequence(_breakCts.Token).Forget();
    }

    public void Exit(GameObject go = null)
    {
        if (_currentState == PlatformState.Preparing)
        {
            _breakCts?.Cancel();
            ResetPlatform();
        }
    }

    private async UniTaskVoid BreakSequence(CancellationToken ct)
    {
        _currentState = PlatformState.Preparing;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTime), cancellationToken: ct);

            await transform.DOShakePosition(_shakeTime, strength: 0.15f, vibrato: 30)
                           .SetLink(gameObject)
                           .ToUniTask(cancellationToken: ct);

            _currentState = PlatformState.Falling;

            if (_collider != null) _collider.enabled = false;

            await transform.DOMoveY(_originPosition.y - _fallDistance, _fallDuration)
                           .SetEase(Ease.InQuad)
                           .SetLink(gameObject)
                           .ToUniTask(cancellationToken: ct);

            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("발판 시퀀스가 취소되어 리셋되었습니다.");
        }
        finally
        {
            _breakCts?.Dispose();
            _breakCts = null;
        }
    }

    private void ResetPlatform()
    {
        transform.DOKill();
        transform.position = _originPosition;

        if (_collider != null) _collider.enabled = true;
        _currentState = PlatformState.Idle;
    }

    private void OnDisable()
    {
        _breakCts?.Cancel();
        _breakCts?.Dispose();
        transform.DOKill();
    }
}