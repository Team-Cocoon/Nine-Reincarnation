using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities;

enum FadeType
{
    Default,
    Directional
}

public class SubSceneLoader : SceneLoader, IFadeEffect
{
    [SerializeField] private FadeType _fadeType;
    public string SubScenePath;

    protected override void Awake()
    {
        base.Awake();

        IncrementLoadCount();
    }

    private async void Start()
    {
        await LoadSubScene();
        DecrementLoadCount();
    }

    public async UniTask LoadSubScene()
    {
        if (LoadSceneCount == 1)
        {
            await FadeOut();
        }
        await LoadLoadingScene();

        await UnloadAllScene();
        int loadSceneCount = LoadSceneCount;

        await UnloadStack();
        await LoadSceneByPath(SubScenePath);

        await UniTask.WaitUntil(() => loadSceneCount == LoadSceneCount, cancellationToken: _token);

        await UnLoadLoadingScene();
        if (LoadSceneCount == 1)
        {
            await FadeIn();
        }
    }

    public async UniTask FadeIn()
    {
        switch (_fadeType)
        {
            case FadeType.Default:
                await UIEventHandler.OnSceneFadeIn_Invoke(true).WithCancellation(_token);
                break;
            case FadeType.Directional:
                await UIEventHandler.OnSceneWipeFadeIn_Invoke(true).WithCancellation(_token);
                break;
        }
    }

    public async UniTask FadeOut()
    {
        switch (_fadeType)
        {
            case FadeType.Default:
                await UIEventHandler.OnSceneFadeOut_Invoke(true).WithCancellation(_token);
                break;
            case FadeType.Directional:
                await UIEventHandler.OnSceneWipeFadeOut_Invoke(true).WithCancellation(_token);
                break;
        }
    }
}
