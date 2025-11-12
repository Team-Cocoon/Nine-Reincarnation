

using Cysharp.Threading.Tasks;
using State;
using State.SceneState;
using StateMachine.SceneStateMachine;
using UnityEngine;
using Utilities;

public class CoreSceneLoader : SceneLoader
{
    private SceneLoader _sceneLoader;
    private SceneStateMachine _sceneStateMachine;
    private SceneStateType _currentSceneState;

    public string TitleScenePath => SceneDataManager.Instance.TitleScene;
    public string StageScenePath => SceneDataManager.Instance.StageCoreScene;
    public string StoryScenePath => SceneDataManager.Instance.StoryCoreScene;
    public string ClearScenePath => SceneDataManager.Instance.ClearScene;

    public SceneStateType CurrentSceneState => _currentSceneState;


    protected override void Awake()
    {
        base.Awake();
        
        _sceneStateMachine = new SceneStateMachine(this);

        _sceneStateMachine.stateChanged += OnStateChanged_LoadCoreScene;
        _sceneStateMachine.Initialize(_sceneStateMachine._titleState);

        GameEventHandler.TitleExcuted += SceneEvent_Title;
        GameEventHandler.StoryExcuted += SceneEvent_Story;
        GameEventHandler.StageExcuted += SceneEvent_Stage;
    }

    private void OnDestroy()
    {
        _sceneStateMachine.stateChanged -= OnStateChanged_LoadCoreScene;

        GameEventHandler.TitleExcuted -= SceneEvent_Title;
        GameEventHandler.StoryExcuted -= SceneEvent_Story;
        GameEventHandler.StageExcuted -= SceneEvent_Stage;
    }

    private void SceneEvent_Title()
    {
        _sceneStateMachine.TransitionState(SceneStateType.Title);
    }

    private void SceneEvent_Story()
    {
        _sceneStateMachine.TransitionState(SceneStateType.Story);
    }

    private void SceneEvent_Stage()
    {
        _sceneStateMachine.TransitionState(SceneStateType.Stage);
    }

    private void SceneEvent_Clear()
    {
        _sceneStateMachine.TransitionState(SceneStateType.Clear);
    }

    private async void OnStateChanged_LoadCoreScene(IState state)
    {
        await LoadCoreScene(state);
    }

    public async UniTask LoadCoreScene(IState state)
    {
        ISceneState sceneState = state as ISceneState;

        _loadSceneCount++;
        int loadSceneCount = _loadSceneCount;
        await LoadLoadingScene();

        await UnloadLastScene();
        await LoadSceneByPath(sceneState.ScenePath);

        Debug.Log(string.Format("Core {0}, {1}", loadSceneCount, _loadSceneCount));
        await UniTask.WaitUntil(() => loadSceneCount == _loadSceneCount, cancellationToken: _token);

        Debug.Log(string.Format("코어 종료"));
        await UnLoadLoadingScene();
        _loadSceneCount--;
    }
}
