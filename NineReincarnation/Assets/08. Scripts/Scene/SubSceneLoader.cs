using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using VContainer;

public enum FadeType
{
    Default,
    Directional
}

public class SubSceneLoader : SceneLoader, IFadeEffect
{
    [Inject] private FadeType _fadeType;
    public string SubScenePath;

    public override void Initialize()
    {
        base.Initialize();
    }

    public async UniTask AddSubScene()
    {
        IncrementLoadCount();

        if (LoadSceneCount == 1)
        {
            await FadeOut();
        }
        await LoadLoadingScene();

        int loadSceneCount = LoadSceneCount;

        await LoadSceneByPath(SubScenePath);

        await UniTask.WaitUntil(() => loadSceneCount == LoadSceneCount, cancellationToken: _cts.Token);

        await UnLoadLoadingScene();
        if (LoadSceneCount == 1)
        {
            await FadeIn();
        }

        DecrementLoadCount();
    }

    public async UniTask ChangeSubScene()
    {
        IncrementLoadCount();

        if (LoadSceneCount == 1)
        {
            await FadeOut();
        }
        await LoadLoadingScene();

        int loadSceneCount = LoadSceneCount;

        await UnloadLastScene();
        await LoadSceneByPath(SubScenePath);

        await UniTask.WaitUntil(() => loadSceneCount == LoadSceneCount, cancellationToken: _cts.Token);

        await UnLoadLoadingScene();
        if (LoadSceneCount == 1)
        {
            await FadeIn();
        }

        DecrementLoadCount();
    }

    public async UniTask LoadSubScene()
    {
        IncrementLoadCount();

        if (LoadSceneCount == 1)
        {
            await FadeOut();
        }
        await LoadLoadingScene();

        int loadSceneCount = LoadSceneCount;

        await UnloadStack();
        await LoadSceneByPath(SubScenePath);

        await UniTask.NextFrame();

        await UniTask.WaitUntil(() => loadSceneCount == LoadSceneCount, cancellationToken: _cts.Token);

        await UnLoadLoadingScene();
        if (LoadSceneCount == 1)
        {
            await FadeIn();
        }

        DecrementLoadCount();
    }

    public async UniTask FadeIn()
    {
        switch (_fadeType)
        {
            case FadeType.Default:
                await UIEventHandler.OnSceneFadeIn_Invoke(true).WithCancellation(_cts.Token);
                break;
            case FadeType.Directional:
                await UIEventHandler.OnSceneWipeFadeIn_Invoke(true).WithCancellation(_cts.Token);
                break;
        }
    }

    public async UniTask FadeOut()
    {
        switch (_fadeType)
        {
            case FadeType.Default:
                await UIEventHandler.OnSceneFadeOut_Invoke(true).WithCancellation(_cts.Token);
                break;
            case FadeType.Directional:
                await UIEventHandler.OnSceneWipeFadeOut_Invoke(true).WithCancellation(_cts.Token);
                break;
        }
    }
}
