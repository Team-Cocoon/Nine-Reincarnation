using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class SpikeObstacle : MonoBehaviour
{
    [Header("Pool Asset")]
    [SerializeField] private SpikePoolSO spikePool; // SO 에셋 할당

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float xRange = 1.5f;
    [SerializeField] private float spawnHeight = 5.0f;

    private CancellationTokenSource _cts;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            SpawnLoop(_cts.Token).Forget();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _cts?.Cancel();
        }
    }

    private async UniTaskVoid SpawnLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                SpawnSpike();
                await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval), cancellationToken: token);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void SpawnSpike()
    {
        // 싱글톤 없이 SO 에셋에서 직접 꺼내옴
        GameObject spikeGo = spikePool.Get();

        float randomX = transform.position.x + UnityEngine.Random.Range(-xRange, xRange);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y + spawnHeight, 0);

        spikeGo.transform.position = spawnPos;
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}