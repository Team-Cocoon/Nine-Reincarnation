using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class StageManager : IDisposable
{
    private readonly SceneDataManager _sceneDataManager;
    private readonly SceneTransitionManager _sceneTransitionManager;
    private readonly SaveManager _saveManager;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    private int _currentStageIndex = 0;
    private int _currentMapIndex = 0;

    [Inject]
    public StageManager(SceneDataManager sceneDataManager, SceneTransitionManager sceneTransitionManager, SaveManager saveManager)
    {
        _sceneDataManager = sceneDataManager;
        _sceneTransitionManager = sceneTransitionManager;
        _saveManager = saveManager;

        SyncCurrentStageIndex();
    }

    private void SyncCurrentStageIndex()
    {
#if UNITY_EDITOR
        string reqPath = CoreBootStrap.RequestedStartScenePath;

        if (!string.IsNullOrEmpty(reqPath))
        {
            int stageIndex = _sceneDataManager.GetStageIndexByPath(reqPath);
            
            if (stageIndex != -1)
            {
                _currentStageIndex = stageIndex;
                
                int mapIndex = _sceneDataManager.GetMapIndexByPath(stageIndex, reqPath);
                if (mapIndex != -1)
                {
                    _currentMapIndex = mapIndex;
                }
                
                Debug.Log($"<color=green>[StageManager]</color> 에디터 시작 위치 동기화 완료! Stage: {_currentStageIndex}, Map: {_currentMapIndex}");
            }
        }
#endif
    }

    public void SetCurrentStageIndex(int stage, int map)
    {
        _currentStageIndex = stage;
        _currentMapIndex = map;
        Debug.Log($"<color=green>[StageManager]</color> 에디터 시작 위치 동기화 완료! Stage: {stage}, Map: {map}");
    }

    public async UniTaskVoid GoToNextMap()
    {
        bool nextStageChanged = _sceneDataManager.NextStage(ref _currentStageIndex, ref _currentMapIndex);
        
        // [클리어 분기]
        if (!_sceneDataManager.HasStage(_currentStageIndex))
        {
            Debug.Log("<color=cyan>[StageManager]</color> 모든 스테이지를 클리어했습니다! 클리어 씬으로 이동합니다.");
            _saveManager.Save(); 

            string clearSceneName = _sceneDataManager.ClearScene;
            
            if (!string.IsNullOrEmpty(clearSceneName))
            {
                List<string> clearSceneList = new List<string> { clearSceneName };
                
                // ★ 수정: 여기서는 _cts.Token 대신 CancellationToken.None을 넘겨줍니다!
                // 현재 스코프가 파괴되어 StageManager가 Dispose되어도 씬 전환은 끝까지 실행됩니다.
                await _sceneTransitionManager.TransitionToScenes(clearSceneList, CancellationToken.None);
            }
            else
            {
                Debug.LogError("[StageManager] SceneDataSO에 ClearScene이 지정되지 않았습니다!");
            }
            
            return; 
        }

        // [일반 맵 이동 분기]
        List<string> targetScenes = _sceneDataManager.GetTargetScenes(_currentStageIndex, _currentMapIndex);

        Debug.Log($"[StageManager] 다음 스테이지/맵으로 이동 준비 중... 스테이지: {_currentStageIndex}, 맵: {_currentMapIndex}");
        _saveManager.Save();

        if (nextStageChanged)
        {
            Debug.Log($"[StageManager] 다음 스테이지({_currentStageIndex})로 이동합니다.");
        }
        else
        {
            Debug.Log($"[StageManager] 다음 맵({_currentMapIndex})으로 이동합니다.");
        }

        // 일반 매핑 중에는 예상치 못한 파괴 시 작업이 취소되는 것이 맞으므로 기존 토큰을 유지해도 좋습니다.
        await _sceneTransitionManager.TransitionToScenes(targetScenes, _cts.Token);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}