using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

public class StageInitiator : IInitiator
{
    private SceneLoader _sceneLoader;
    private string _scenePath;

    [Inject]
    public StageInitiator(SceneLoader sceneLoader, string scenePath)
    {
        _sceneLoader = sceneLoader;
        _scenePath = scenePath;
    }

    public async UniTask GameInitialize(CancellationToken token)
    {
        await ExcuteInit(token);
    }

    private async UniTask ExcuteInit(CancellationToken token)
    {
        await _sceneLoader.LoadSceneByPath(_scenePath, token);
    }
}
