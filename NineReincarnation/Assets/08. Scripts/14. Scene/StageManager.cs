using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

// MonoBehaviour를 제거하고, 메모리 해제를 위해 IDisposable을 달아줍니다.
public class StageManager : IDisposable
{
    private readonly SceneDataManager _sceneDataManager;
    private readonly SceneTransitionManager _sceneTransitionManager;
    private readonly SaveManager _saveManager;
    // 이 매니저 전용 취소 토큰 소스
    private CancellationTokenSource _cts = new CancellationTokenSource();

    // 현재 상태 (필요하다면 GameManager 등에서 관리해도 됩니다)
    private int _currentStageIndex = 0;
    private int _currentMapIndex = 0;

    // VContainer가 알아서 매니저들을 생성자로 넣어줍니다!
    [Inject]
    public StageManager(SceneDataManager sceneDataManager, SceneTransitionManager sceneTransitionManager, SaveManager saveManager)
    {
        _sceneDataManager = sceneDataManager;
        _sceneTransitionManager = sceneTransitionManager;
        _saveManager = saveManager;
    }

    public async UniTaskVoid GoToNextMap()
    {
        bool nextStageChanged = _sceneDataManager.NextStage(ref _currentStageIndex, ref _currentMapIndex);
        
        _saveManager.Save();
        List<string> targetScenes = _sceneDataManager.GetTargetScenes(_currentStageIndex, _currentMapIndex);

        if (nextStageChanged)
        {
            Debug.Log($"[StageManager] 다음 스테이지({_currentStageIndex})로 이동합니다.");
        }
        else
        {
            Debug.Log($"[StageManager] 다음 맵({_currentMapIndex})으로 이동합니다.");
        }

        // _cts.Token을 넘겨서 매니저가 파괴될 때 전환 작업도 취소되도록 안전장치 마련
        await _sceneTransitionManager.TransitionToScenes(targetScenes, _cts.Token);
    }

    // 클래스가 파괴되거나 컨테이너가 Dispose될 때 호출됨
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}