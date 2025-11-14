using Cysharp.Threading.Tasks;
using State.SceneState;
using UnityEngine;
using Utilities;
using VContainer;

public class SubSceneLoader : SceneLoader, IFadeEffect
{
    [Inject] private string subScenePath;

    protected override void Awake()
    {
        base.Awake();
       
        _loadSceneCount++;
    }

    private async void Start()
    {
        await LoadSubScene();
        _loadSceneCount--;
    }

    public async UniTask LoadSubScene()
    {
        if (_loadSceneCount == 1)
        {
            await FadeOut();
        }
        await LoadLoadingScene();

        int loadSceneCount = _loadSceneCount;

        await UnloadAllScene();
        await LoadSceneByPath(subScenePath);

        Debug.Log(string.Format("Sub {0}, {1}", loadSceneCount, _loadSceneCount));
        await UniTask.WaitUntil(() => loadSceneCount == _loadSceneCount, cancellationToken: _token);

        Debug.Log(string.Format("서브 종료"));
        await UnLoadLoadingScene();
        if (_loadSceneCount == 1)
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
