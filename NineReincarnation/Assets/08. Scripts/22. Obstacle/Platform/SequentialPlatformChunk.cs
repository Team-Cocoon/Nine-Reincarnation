using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;

public class SequentialPlatformChunk : MonoBehaviour
{
    private List<List<SequentialPlatform>> _platformGroups = new List<List<SequentialPlatform>>();
    private List<SequentialPlatform> _tempRegisterList = new List<SequentialPlatform>();

    [SerializeField] private float stepDurationSeconds = 1.5f;

    public void RegisterSequentialPlatform(SequentialPlatform platform)
    {
        _tempRegisterList.Add(platform);
    }

    private async UniTaskVoid Start()
    {
        // 모든 자식 플랫폼이 등록될 때까지 한 프레임 대기
        await UniTask.NextFrame();

        if (_tempRegisterList.Count == 0) return;

        // 1. 인덱스 순으로 정렬 후, 같은 인덱스를 가진 애들끼리 그룹화(GroupBy)
        _platformGroups = _tempRegisterList
            .GroupBy(p => p.sequenceIndex)
            .OrderBy(g => g.Key)
            .Select(g => g.ToList())
            .ToList();

        var ct = this.GetCancellationTokenOnDestroy();

        try
        {
            await RunPlatformSequence(ct);
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴 시 발생하는 자연스러운 예외
        }
    }

    private async UniTask RunPlatformSequence(CancellationToken ct)
    {
        int currentGroupIndex = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            // 현재 순서의 그룹 추출
            List<SequentialPlatform> currentGroup = _platformGroups[currentGroupIndex];

            // 그룹 내 모든 플랫폼 동시 활성화
            foreach (var platform in currentGroup)
            {
                platform.SetState(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(stepDurationSeconds), cancellationToken: ct);

            // 그룹 내 모든 플랫폼 동시 비활성화
            foreach (var platform in currentGroup)
            {
                platform.SetState(false);
            }

            // 다음 그룹으로 인덱스 이동
            currentGroupIndex = (currentGroupIndex + 1) % _platformGroups.Count;
        }
    }
}
