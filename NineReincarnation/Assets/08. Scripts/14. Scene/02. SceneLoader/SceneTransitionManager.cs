using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

/// <summary>
/// 씬 간 트랜지션을 관리하는 매니저 클래스
/// </summary>
public class SceneTransitionManager
{
    private const double MinimumLoadingSeconds = 1.0;

    private readonly FadeUI _fadeUI;
    private readonly GameObject _loadingScreen;
    private readonly SceneLoader _sceneLoader;
    private readonly TransitionTaskRegistry _taskRegistry;
    private readonly SceneDataManager _sceneDataManager;
    private readonly LoadingUIStarter _loadingUI;
    private readonly SemaphoreSlim _transitionLock = new SemaphoreSlim(1, 1);

    [Inject]
    public SceneTransitionManager(FadeUI fadeUI, GameObject loadingScreen, SceneLoader sceneLoader, TransitionTaskRegistry taskRegistry, SceneDataManager sceneDataManager)
    {
        _fadeUI = fadeUI;
        _loadingScreen = loadingScreen;
        _sceneLoader = sceneLoader;
        _taskRegistry = taskRegistry;
        _sceneDataManager = sceneDataManager;
        _loadingUI = loadingScreen.GetComponent<LoadingUIStarter>();
    }

    public async UniTask TransitionToScenes(List<string> requestedScenes, CancellationToken token = default, bool enableFadeInOut = true)
    {
        // Serialize transitions so a second request cannot move the same doors.
        var targets = new List<string>(requestedScenes);
        var lifetimeToken = _loadingScreen.GetCancellationTokenOnDestroy();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, lifetimeToken);
        await _transitionLock.WaitAsync(linkedCts.Token);
        try
        {
            await ExecuteTransitionAsync(targets, linkedCts.Token, lifetimeToken, enableFadeInOut);
        }
        finally
        {
            _transitionLock.Release();
        }
    }

    private bool ShouldUseDoors(List<string> requestedScenes)
    {
        // Both initial title entry and returning to title retain the original UI.
        if (requestedScenes.Contains(_sceneDataManager.TitleScene)) return false;
        return requestedScenes.Any(path => _sceneDataManager.GetStageIndexByPath(path) >= 0)
            || (requestedScenes.Contains(_sceneDataManager.ClearScene)
                && _sceneLoader.LoadedScenes.Contains(_sceneDataManager.StageCoreScene));
    }

    private async UniTask ExecuteTransitionAsync(List<string> requestedScenes, CancellationToken token,
        CancellationToken lifetimeToken, bool enableFadeInOut)
    {
        bool useDoors = ShouldUseDoors(requestedScenes) && _loadingUI != null && _loadingUI.HasDoors;
        AudioManager.Instance?.StopAllSfx();
        _taskRegistry.ClearTasks();
        try
        {
            // Stage doors replace the fade, including callers that disable fades.
            if (!useDoors && enableFadeInOut)
                await _fadeUI.UIEvent_FadeOut().AsyncWaitForCompletion().AsUniTask();

            token.ThrowIfCancellationRequested();
            if (_loadingUI != null) await _loadingUI.ShowAsync(useDoors, token);
            else _loadingScreen.SetActive(true);

            // Count real time after the doors finish their rebound and settle closed.
            // Scene loading runs inside this interval, not before an extra fixed delay.
            var loadingTimer = System.Diagnostics.Stopwatch.StartNew();

            if (useDoors) _fadeUI.CloseUI();
            foreach (var scene in requestedScenes)
                Debug.Log($"[SceneTransition] 현재 로드되어야 하는 씬: {scene}");

            await ApplySceneChangesAsync(requestedScenes, token);
            await _taskRegistry.WaitAllTasksAsync().AttachExternalCancellation(token);
            await UniTask.WaitUntil(() => loadingTimer.Elapsed.TotalSeconds >= MinimumLoadingSeconds,
                cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("<color=yellow>[SceneTransition]</color> 씬 전환 작업 취소됨.");
        }
        catch (Exception e)
        {
            Debug.LogError("<color=red>[SceneTransition]</color> 씬 전환 중 오류 발생 상세 로그:");
            Debug.LogException(e);
        }
        finally
        {
            // A load cancellation still opens from the current pose. Only teardown
            // of BaseScene itself cancels cleanup, so the UI cannot remain stuck.
            if (!lifetimeToken.IsCancellationRequested)
            {
                if (_loadingUI != null) await _loadingUI.HideAsync(lifetimeToken);
                else _loadingScreen.SetActive(false);

                if (!useDoors && enableFadeInOut)
                    await _fadeUI.UIEvent_FadeIn().AsyncWaitForCompletion().AsUniTask();
            }
        }
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
