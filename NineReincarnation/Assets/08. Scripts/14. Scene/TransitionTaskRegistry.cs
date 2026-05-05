using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionTaskRegistry
{
    private List<UniTask> _loadingTasks = new List<UniTask>();

    // 로딩 시작 시 목록 초기화
    public void ClearTasks() => _loadingTasks.Clear();

    // 새로 로드된 씬들이 자신의 작업을 여기에 등록
    public void RegisterTask(UniTask task) => _loadingTasks.Add(task);

    // 매니저가 이 함수를 호출해 모든 작업이 끝날 때까지 대기
    public async UniTask WaitAllTasksAsync()
    {
        if (_loadingTasks.Count > 0)
        {
            await UniTask.WhenAll(_loadingTasks);
        }
    }
}
