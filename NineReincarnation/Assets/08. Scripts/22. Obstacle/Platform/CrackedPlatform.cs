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
    [SerializeField] private float _respawnTime = 3.0f;

    private PlatformState _currentState = PlatformState.Idle;
    private CancellationTokenSource _breakCts;
    private Vector3 _originPosition;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _originPosition = transform.position;
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // ✨ 추가
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
            // 밟고 버티는 시간
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTime), cancellationToken: ct);

            // 흔들림 연출
            await transform.DOShakePosition(_shakeTime, strength: 0.15f, vibrato: 30)
                           .SetLink(gameObject)
                           .ToUniTask(cancellationToken: ct);

            _currentState = PlatformState.Falling;

            // 추락 시작할 때 충돌체 끄기
            if (_collider != null) _collider.enabled = false;

            // 추락 연출
            await transform.DOMoveY(_originPosition.y - _fallDistance, _fallDuration)
                           .SetEase(Ease.InQuad)
                           .SetLink(gameObject)
                           .ToUniTask(cancellationToken: ct);

            if (_spriteRenderer != null) _spriteRenderer.enabled = false;

            // 정한 시간만큼 부활 대기
            await UniTask.Delay(TimeSpan.FromSeconds(_respawnTime), cancellationToken: ct);

            // 리셋
            ResetPlatform();
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
        if (_spriteRenderer != null) _spriteRenderer.enabled = true; 
        
        _currentState = PlatformState.Idle;
    }

    private void OnDisable()
    {
        _breakCts?.Cancel();
        _breakCts?.Dispose();
        transform.DOKill();
    }
}