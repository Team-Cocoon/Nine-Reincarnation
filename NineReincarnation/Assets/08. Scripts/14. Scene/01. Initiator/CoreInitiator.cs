using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class CoreInitiator : IInitiator
{
    private FadeUI _fadeScreen;
    private GameObject _loadingScreen;
    private CoreSceneLoader _sceneLoader;
    private string _scenePath;

    [Inject]
    public CoreInitiator(GameObject loadingScreen, CoreSceneLoader sceneLoader, string scenePath, FadeUI fadeUI)
    {
        _loadingScreen = loadingScreen;
        _sceneLoader = sceneLoader;
        _scenePath = scenePath;
        _fadeScreen = fadeUI;
    }

    public async UniTask GameInitialize(CancellationToken token)
    {
        await ExcuteInit(token);
    }

    private async UniTask ExcuteInit(CancellationToken token)
    {

        Debug.Log($"<color=yellow>[SceneLoader]</color> 씬 로드 시도: '{_scenePath}'");

        using (var Loding = new LoadingUIStarter(_loadingScreen))
        {
            await _sceneLoader.LoadSceneByPath(_scenePath, token);
        }
        await _fadeScreen.UIEvent_FadeIn();
    }
}
