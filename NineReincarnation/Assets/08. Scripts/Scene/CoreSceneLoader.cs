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

        bool isLoad = false;

        await LoadLoadingScene();

        await UnloadLastScene();
        await LoadSceneByPath(sceneState.ScenePath);
        //LoadSceneByPath(sceneState.ScenePath);로 실행되는 씬에서 Awake에서 비동기 함수가 끝날때까지 대기해야함

        await UniTask.WaitUntil(() => isLoad == false, cancellationToken: _token);
        await UnLoadLoadingScene();
    }
}
