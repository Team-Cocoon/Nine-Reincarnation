using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

public enum SceneStateType
{
    None,
    Title,
    Stage,
    Clear
}

public class CoreSceneLoader : SceneLoader
{
    [Inject] private SceneDataManager _sceneDataManager;
    [Inject] private SaveManager      _saveManager;
    private SceneStateType    _currentSceneState;

    private string _titleScenePath => _sceneDataManager.TitleScene;
    private string _stageScenePath => _sceneDataManager.StageCoreScene;
    private string _clearScenePath => _sceneDataManager.ClearScene;

    public SceneStateType CurrentSceneState => _currentSceneState;

    public override void Dispose()
    {
        base.Dispose();
    }

    public async UniTask SceneEvent_Title(CancellationToken token)
    {
        await LoadSceneByPath(_titleScenePath, token);
    }

    public async UniTask SceneEvent_Stage(CancellationToken token)
    {
        await LoadSceneByPath(_stageScenePath, token);
    }

    public async UniTask SceneEvent_Clear(CancellationToken token)
    {
        await LoadSceneByPath(_clearScenePath, token);
    }
}
