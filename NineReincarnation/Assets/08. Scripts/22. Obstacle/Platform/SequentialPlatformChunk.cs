using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SequentialPlatformChunk : MonoBehaviour
{
    private List<SequentialPlatform> _platformList = new List<SequentialPlatform>();
    [SerializeField] private float stepDurationSeconds = 1.0f;

    public void RegisterSequentialPlatform(SequentialPlatform platform)
    {
        _platformList.Add(platform);
    }

    private async UniTaskVoid Start()
    {
        await UniTask.NextFrame();

        if (_platformList.Count == 0) return;

        _platformList.Sort((a, b) => a.sequenceIndex.CompareTo(b.sequenceIndex));

        var ct = this.GetCancellationTokenOnDestroy();

        try
        {
            await RunPlatformSequence(ct);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async UniTask RunPlatformSequence(CancellationToken ct)
    {
        int currentIndex = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            SequentialPlatform current = _platformList[currentIndex];

            current.SetState(true);

            await UniTask.Delay(TimeSpan.FromSeconds(stepDurationSeconds), cancellationToken: ct);

            current.SetState(false);

            currentIndex = (currentIndex + 1) % _platformList.Count;
        }
    }
}
