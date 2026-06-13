using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class SpikeObstacle : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpikePoolSO[] spikePools;

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float xRange = 1.5f;
    [SerializeField] private float spawnHeight = 5.0f;

    private CancellationTokenSource _cts;
    private static readonly int TreeShakeHash = Animator.StringToHash("Tree_Shake");
    private static readonly int TreeIdleHash = Animator.StringToHash("Tree_Idle");

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
                animator.Play(TreeShakeHash);
                animator.Update(0f);
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float shakeDuration = stateInfo.length;

                SpawnSpike();

                await UniTask.Delay(TimeSpan.FromSeconds(shakeDuration), cancellationToken: token);

                animator.Play(TreeIdleHash);

                float remainingDelay = spawnInterval - shakeDuration;
                if (remainingDelay > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(remainingDelay), cancellationToken: token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            ResetToIdle();
        }
        catch (Exception e)
        {
            // [수정됨] 풀링 에러 등 알 수 없는 에러로 루프가 멈추고 나무가 굳는 현상 방지
            Debug.LogError($"SpawnLoop Error: {e.Message}");
            ResetToIdle();
        }
    }

    private void SpawnSpike()
    {
        if (spikePools == null || spikePools.Length == 0)
        {
            Debug.LogWarning("SpikePools 배열이 비어 있습니다!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, spikePools.Length);
        SpikePoolSO selectedPool = spikePools[randomIndex];

        if (selectedPool == null) return;

        GameObject spikeGo = selectedPool.Get();

        float randomX = transform.position.x + UnityEngine.Random.Range(-xRange, xRange);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y + spawnHeight, 0);

        // [수정됨] 1. 위치를 최상단으로 먼저 옮깁니다.
        spikeGo.transform.position = spawnPos;

        // [수정됨] 2. 위치가 완전히 세팅된 후 활성화해야 이전 위치에서의 충돌 버그를 막을 수 있습니다.
        spikeGo.SetActive(true);
    }

    private void ResetToIdle()
    {
        if (animator != null)
        {
            animator.Play(TreeIdleHash);
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}