using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;
using Player.Controller;

public class Thorn : MonoBehaviour, ICollidable
{
    [Header("Movement Settings")]
    [SerializeField] private float riseDuration = 0.2f;   
    [SerializeField] private float stayDuration = 0.5f;   
    [SerializeField] private float fallDuration = 0.3f;  
    [SerializeField] private float riseDistance = 1.5f;   
    [SerializeField] private float cycleInterval = 1.5f;  

    private CancellationTokenSource _cts;
    private Vector3 _startPosition;
    private bool _isInitialized;

    private void Awake()
    {
        _startPosition = transform.position;
        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (!_isInitialized) _startPosition = transform.position;

        transform.DOKill();
        transform.position = _startPosition;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        ThornLoop(_cts.Token).Forget();
    }

    private void OnDisable()
    {
        transform.DOKill();
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTaskVoid ThornLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                Sequence thornSequence = DOTween.Sequence();

                thornSequence.Append(transform.DOMoveY(_startPosition.y + riseDistance, riseDuration).SetEase(Ease.OutBack))
                             .AppendInterval(stayDuration)
                             .Append(transform.DOMoveY(_startPosition.y, fallDuration).SetEase(Ease.InQuad));

                await thornSequence.SetLink(gameObject)
                                  .WithCancellation(token);

                float totalMotionTime = riseDuration + stayDuration + fallDuration;

                float remainingDelay = cycleInterval - totalMotionTime;

                if (remainingDelay > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(remainingDelay), cancellationToken: token);
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    public void Enter(GameObject go = null)
    {
        if (go != null && go.TryGetComponent<PlayerController>(out var player))
        {
            player.Dead();
        }
    }

    public void Exit(GameObject go = null) { }
}