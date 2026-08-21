using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Player.Controller;

public class Spike : MonoBehaviour, ICollidable
{
    private SpikePoolSO _originPool;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float fallDuration = 2f; // 떨어지는 시간
    [SerializeField] private float fallDistance = 15f; // 떨어질 거리

    private CancellationTokenSource _cts;

    public void SetPool(SpikePoolSO pool) => _originPool = pool;

    private void OnEnable()
    {
        transform.DOKill();

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        StartFalling(_cts.Token).Forget();
    }

    private void OnDisable()
    {
        transform.DOKill();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTaskVoid StartFalling(CancellationToken token)
    {
        try
        {
            await transform.DOMoveY(transform.position.y - fallDistance, fallDuration)
                .SetEase(Ease.InQuad) // 살짝 가속도 붙는 느낌
                .SetLink(gameObject)
                .WithCancellation(token);

            ReturnToPool();
        }
        catch (System.OperationCanceledException) { }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_groundLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf && _originPool != null)
        {
            _originPool.Release(gameObject);
        }
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