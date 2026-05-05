using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

/// <summary>
/// 씬 간 트랜지션을 관리하는 매니저 클래스
/// </summary>
public class SceneTransitionManager
{
    private readonly FadeUI _fadeUI;
    private readonly GameObject _loadingScreen;
    private readonly SceneLoader _sceneLoader;
    private readonly TransitionTaskRegistry _taskRegistry;

    [Inject]
    public SceneTransitionManager(FadeUI fadeUI, GameObject loadingScreen, SceneLoader sceneLoader, TransitionTaskRegistry taskRegistry)
    {
        _fadeUI = fadeUI;
        _loadingScreen = loadingScreen;
        _sceneLoader = sceneLoader;
        _taskRegistry = taskRegistry;
    }

    public async UniTask TransitionToScenes(List<string> requestedScenes, CancellationToken token = default)
    {
        await _fadeUI.UIEvent_FadeOut();

        _taskRegistry.ClearTasks();
        
        using (var loading = new LoadingUIStarter(_loadingScreen))
        {
            try
            {
                await ApplySceneChangesAsync(requestedScenes, token);

                await _taskRegistry.WaitAllTasksAsync();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("<color=yellow>[SceneTransition]</color> 씬 전환 작업 취소됨.");
            }
            catch (Exception e)
            {
                // [수정] e.Message 대신 예외 객체 자체를 로그로 찍습니다.
                Debug.LogError($"<color=red>[SceneTransition]</color> 씬 전환 중 오류 발생 상세 로그:");
                Debug.LogException(e); 
            }
        }

        await _fadeUI.UIEvent_FadeIn();
    }

    private async UniTask ApplySceneChangesAsync(List<string> requestedScenes, CancellationToken token)
    {
        var currentScenes = _sceneLoader.LoadedScenes.ToList();

        // 차집합 계산 (지울 씬, 새로 열 씬)
        var scenesToUnload = currentScenes.Except(requestedScenes).ToList();
        var scenesToLoad = requestedScenes.Except(currentScenes).ToList();

        // 불필요한 씬 역순 언로드
        for (int i = currentScenes.Count - 1; i >= 0; i--)
        {
            string scenePath = currentScenes[i];
            if (scenesToUnload.Contains(scenePath))
            {
                await _sceneLoader.UnloadSceneByPath(scenePath, token);
            }
        }

        // 새로운 씬 로드
        foreach (var scenePath in scenesToLoad)
        {
            await _sceneLoader.LoadSceneByPath(scenePath, token);
        }
    }
}