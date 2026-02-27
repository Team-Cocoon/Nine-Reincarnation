using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class CoreInitiator : IInitiator
{
    [Inject] private GameObject _loadingScreen;
    [Inject] private CoreSceneLoader _sceneLoader;
    [Inject] private string _scenePath;

    public async UniTask GameInitialize(CancellationToken token)
    {
        await ExcuteInit(token);
    }

    private async UniTask ExcuteInit(CancellationToken token)
    {
        using (var Loding = new LoadingUIStarter(_loadingScreen))
        {
#if UNITY_EDITOR
            //BaseSCene부터 열었다면
            if (CoreBootStrap.RequestedStartSceneName == "BaseScene")
            {
                //그대로 타이틀 씬 열기 진행
                await _sceneLoader.LoadSceneByPath(CoreBootStrap.RequestedStartSceneName, token);
            }
            else
            {
                await _sceneLoader.LoadSceneByPath(_scenePath, token);
            }
#else 
            await _sceneLoader.LoadSceneByPath(_scenePath, token);
#endif
        }
    }
}
