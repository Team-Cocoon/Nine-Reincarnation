using System.Collections.Generic;
using State.SceneState;
using StateMachine.SceneStateMachine;
using UnityEngine;


namespace Manager
{
    public class SceneStateManager : MonoBehaviour
    {
        public static SceneStateManager Instance { get; private set; }

        private SceneStateMachine _sceneStateMachine;
        private SceneState _currentSceneState;
        private string _currentScenePath;

        private List<string> _stageSubScenes => SceneDataManager.Instance.GetStageSubScene(0);
        private List<string> _storySubScenes => SceneDataManager.Instance.GetStorySubScene(0);

        public SceneState CurrentSceneState => _currentSceneState;
        public SceneStateMachine SceneStateMachine => _sceneStateMachine;
        public string TitleScenePath => SceneDataManager.Instance.TitleScene;
        public string StageScenePath => SceneDataManager.Instance.StageCoreScene;
        public string StoryScenePath => SceneDataManager.Instance.StoryCoreScene;
        public string ClearScenePath => SceneDataManager.Instance.ClearScene;


        private void Awake()
        {
            _currentScenePath = "";
            Instance = this;
        }

        private void Start()
        {
            _sceneStateMachine = new SceneStateMachine(this);

            _sceneStateMachine.Initialize(_sceneStateMachine._titleState);

            GameEvent_ToTitle();

            SceneEventHandler.OnSceneChanged += ChandedScene;
            GameEventHandler.TitleExcuted += GameEvent_ToTitle;
            GameEventHandler.StoryExcuted += GameEvent_ToStory;
            GameEventHandler.StageExcuted += GameEvent_ToStage;
            GameEventHandler.GameClearExcuted += GameEvent_ToClear;
            SceneEventHandler.SceneExited += HandleSceneExited;
        }


        private void OnDestroy()
        {
            SceneEventHandler.OnSceneChanged -= ChandedScene;
            GameEventHandler.TitleExcuted -= GameEvent_ToTitle;
            GameEventHandler.StoryExcuted -= GameEvent_ToStory;
            GameEventHandler.StageExcuted -= GameEvent_ToStage;
            GameEventHandler.GameClearExcuted -= GameEvent_ToClear;
            SceneEventHandler.SceneExited -= HandleSceneExited;
        }

        private void GameEvent_ToTitle()
        {
            _currentSceneState = SceneState.Title;

            GameEvent_TransitionScene(_currentSceneState);
        }

        private void GameEvent_ToStory()
        {
            _currentSceneState = SceneState.Story;

            GameEvent_TransitionScene(_currentSceneState);
        }

        private void GameEvent_ToStage()
        {
            _currentSceneState = SceneState.Stage;

            GameEvent_TransitionScene(_currentSceneState);
        }

        private void GameEvent_ToClear()
        {
            _currentSceneState = SceneState.Clear;

            GameEvent_TransitionScene(_currentSceneState);
        }

        private void GameEvent_TransitionScene(SceneState state)
        {
            ISceneState sceneState = _sceneStateMachine.GetStateByEnum(state);
            ChangeScene(sceneState);
        }

        private void HandleSceneExited()
        {
            _sceneStateMachine.TransitionState(_currentSceneState);
        }

        private void ChangeScene(ISceneState state)
        {
            string scenePath = state.ScenePath;
            switch (state.StateType)
            {
                case SceneState.Stage:
                    Debug.Log(string.Format("로딩 스테이지 : {0}, 현재 스테이지 : {1} : {2}", scenePath, _currentScenePath, state.StateType.ToString()));

                    List<string> _subScenes = _stageSubScenes.GetRange(0, 2);

                    SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(scenePath, _currentScenePath, _subScenes);
                    break;
                case SceneState.Story:
                    Debug.Log(string.Format("로딩 스테이지 : {0}, 현재 스테이지 : {1} : {2}", scenePath, _currentScenePath, state.StateType.ToString()));
                    SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(scenePath, _currentScenePath, _storySubScenes);
                    break;
                default:
                    Debug.Log(string.Format("로딩 스테이지 : {0}, 현재 스테이지 : {1} : {2}", scenePath, _currentScenePath, state.StateType.ToString()));
                    SceneEventHandler.SceneStateChanged_Invoke(scenePath, _currentScenePath);
                    break;
            }
        }

        public void ChandedScene(string scenePath)
        {
            _currentScenePath = scenePath;
        }
    }
}