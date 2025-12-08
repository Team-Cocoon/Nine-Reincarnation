using Cysharp.Threading.Tasks;
using State;
using State.SceneState;
using StateMachine.SceneStateMachine;
using UnityEngine;
using Utilities;

public class CoreSceneLoader : SceneLoader, IFadeEffect
{
    private SceneStateMachine _sceneStateMachine;
    private SceneStateType _currentSceneState;

    public string TitleScenePath => _sceneDataManager.TitleScene;
    public string StageScenePath => _sceneDataManager.StageCoreScene;
    public string StoryScenePath => _sceneDataManager.StoryCoreScene;
    public string ClearScenePath => _sceneDataManager.ClearScene;

    public SceneStateType CurrentSceneState => _currentSceneState;

    public override void Initialize()
    {
        base.Initialize();
        _sceneStateMachine = new SceneStateMachine(this);
    }

    public override void Start()
    {
        base.Start();

        _sceneStateMachine.stateChanged += OnStateChanged_LoadCoreScene;
        _sceneStateMachine.Initialize(_sceneStateMachine._titleState);

        GameEventHandler.TitleExcuted += SceneEvent_Title;
        GameEventHandler.StoryExcuted += SceneEvent_Story;
        GameEventHandler.StageExcuted += SceneEvent_Stage;
        GameEventHandler.GameClearExcuted += SceneEvent_Clear;
    }

    public override void Dispose()
    {
        base.Dispose();

        _sceneStateMachine.stateChanged -= OnStateChanged_LoadCoreScene;

        GameEventHandler.TitleExcuted -= SceneEvent_Title;
        GameEventHandler.StoryExcuted -= SceneEvent_Story;
        GameEventHandler.StageExcuted -= SceneEvent_Stage;
        GameEventHandler.GameClearExcuted -= SceneEvent_Clear;
    }

    private void SceneEvent_Title()
    {
        _sceneStateMachine.TransitionState(SceneStateType.Title);
    }

    private void SceneEvent_Story()
    {
        _saveManager.SetState(GameState.Stoty);
        _sceneStateMachine.TransitionState(SceneStateType.Story);
    }

    private void SceneEvent_Stage()
    {
        _saveManager.SetState(GameState.Stage);
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

        IncrementLoadCount();

        //페이드 아웃
        await FadeOut();

        int loadSceneCount = LoadSceneCount;
        await LoadLoadingScene();

        await UnloadStack();

        await UnloadAllScene();

        await LoadSceneByPath(sceneState.ScenePath);

        await UniTask.WaitUntil(() => loadSceneCount == LoadSceneCount, cancellationToken: _cts.Token);

        await UnLoadLoadingScene();

        //페이드 인 순서
        _currentSceneState = sceneState.StateType;
        await FadeIn();

        DecrementLoadCount();
    }

    //밝아지는 효과
    public async UniTask FadeIn()
    {
        switch (_currentSceneState)
        {
            case SceneStateType.Story:
            case SceneStateType.Title:
                await UIEventHandler.OnSceneFadeIn_Invoke(true).WithCancellation(_cts.Token);
                break;
            default:
                await UIEventHandler.OnSceneWipeFadeIn_Invoke(true).WithCancellation(_cts.Token);
                break;
        }
    }

    //어두워지는 효과
    public async UniTask FadeOut()
    {
        switch (_currentSceneState)
        {
            case SceneStateType.Story:
            case SceneStateType.Title:
                await UIEventHandler.OnSceneFadeOut_Invoke(true).WithCancellation(_cts.Token);
                break;
            default:
                await UIEventHandler.OnSceneWipeFadeOut_Invoke(true).WithCancellation(_cts.Token);
                break;
        }
    }
}
