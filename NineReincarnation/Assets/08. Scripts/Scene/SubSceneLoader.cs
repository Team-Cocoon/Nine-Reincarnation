using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using State.SceneState;
using UnityEngine;
using Utilities;
using VContainer;

public class SubSceneLoader : SceneLoader, IFadeEffect
{
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
        await UIEventHandler.OnSceneWipeFadeIn_Invoke(true).WithCancellation(_token);
    }

    public async UniTask FadeOut()
    {
        await UIEventHandler.OnSceneWipeFadeOut_Invoke(true).WithCancellation(_token);
    }
}
