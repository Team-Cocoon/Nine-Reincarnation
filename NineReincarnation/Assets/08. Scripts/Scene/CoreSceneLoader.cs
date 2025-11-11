using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using State;
using State.SceneState;
using StateMachine.SceneStateMachine;
using Utilities;

public class CoreSceneLoader : SceneLoader
{
    private SceneStateMachine _sceneStateMachine;
    private SceneStateType _currentSceneState;

    public string TitleScenePath => SceneDataManager.Instance.TitleScene;
    public string StageScenePath => SceneDataManager.Instance.StageCoreScene;
    public string StoryScenePath => SceneDataManager.Instance.StoryCoreScene;
    public string ClearScenePath => SceneDataManager.Instance.ClearScene;

    protected override void Awake()
    {
        base.Awake();
        
        _sceneStateMachine = new SceneStateMachine(this);

        _sceneStateMachine.stateChanged += OnStateChanged_LoadCoreScene;
        _sceneStateMachine.Initialize(_sceneStateMachine._titleState);
    }

    private void OnDestroy()
    {
        _sceneStateMachine.stateChanged -= OnStateChanged_LoadCoreScene;
    }

    private async void OnStateChanged_LoadCoreScene(IState state)
    {
        await LoadCoreScene(state);
    }

    public async UniTask LoadCoreScene(IState state)
    {
        ISceneState sceneState = state as ISceneState;

        await LoadSceneByPath(sceneState.ScenePath);
    }
}
