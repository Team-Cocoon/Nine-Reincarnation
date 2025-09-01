using System.Collections.Generic;
using State.SceneState;
using StateMachine.SceneStateMachine;
using UnityEngine;


namespace Manager
{
    public class SceneStateManager : MonoBehaviour
    {
        private List<string> _stageSubScenes => SceneDataManager.Instance.GetStageSubScene(0);

        private List<string> _storySubScenes => SceneDataManager.Instance.GetStorySubScene(0);

        private SceneStateMachine _sceneStateMachine;

        private SceneState _nextSceneState;

        private string _currentScenePath;

        public SceneStateMachine SceneStateMachine => _sceneStateMachine;
        public string TitleScenePath => SceneDataManager.Instance.TitleScene;
        public string StageScenePath => SceneDataManager.Instance.StageCoreScene;
        public string StoryScenePath => SceneDataManager.Instance.StoryCoreScene;
        public string ClearScenePath => SceneDataManager.Instance.ClearScene;


        private void Awake()
        {
            _currentScenePath = "";
        }

        private void Start()
        {
            _sceneStateMachine = new SceneStateMachine(this);

            _sceneStateMachine.Initialize(_sceneStateMachine._titleState);

            GameEvent_ToTitle();
            GameEventHandler.TitleExcuted += GameEvent_ToTitle;
            GameEventHandler.StoryExcuted += GameEvent_ToStory;
            GameEventHandler.StageExcuted += GameEvent_ToStage;
            GameEventHandler.GameClearExcuted += GameEvent_ToClear;
            SceneEventHandler.SceneExited += HandleSceneExited;
        }


        private void OnDestroy()
        {
            GameEventHandler.TitleExcuted -= GameEvent_ToTitle;
            GameEventHandler.StoryExcuted -= GameEvent_ToStory;
            GameEventHandler.StageExcuted -= GameEvent_ToStage;
            GameEventHandler.GameClearExcuted -= GameEvent_ToClear;
            SceneEventHandler.SceneExited -= HandleSceneExited;
        }

        private void GameEvent_ToTitle()
        {
            _nextSceneState = SceneState.Title;

            GameEvent_TransitionScene(_nextSceneState);
        }

        private void GameEvent_ToStory()
        {
            _nextSceneState = SceneState.Story;

            GameEvent_TransitionScene(_nextSceneState);
        }

        private void GameEvent_ToStage()
        {
            _nextSceneState = SceneState.Stage;

            GameEvent_TransitionScene(_nextSceneState);
        }

        private void GameEvent_ToClear()
        {
            _nextSceneState = SceneState.Clear;

            GameEvent_TransitionScene(_nextSceneState);
        }

        private void GameEvent_TransitionScene(SceneState state)
        {
            ISceneState sceneState = _sceneStateMachine.GetStateByEnum(state);
            ChangeScene(sceneState);
        }

        private void HandleSceneExited()
        {
            _sceneStateMachine.TransitionState(_nextSceneState);
        }

        private void ChangeScene(ISceneState state)
        {
            string scenePath = state.ScenePath;

            switch (state.StateType)
            {
                case SceneState.Stage:
                    SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(scenePath, _currentScenePath, _stageSubScenes);
                    break;
                case SceneState.Story:
                    SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(scenePath, _currentScenePath, _storySubScenes);
                    break;
                default:
                    SceneEventHandler.SceneStateChanged_Invoke(scenePath, _currentScenePath);
                    break;
            }

            _currentScenePath = scenePath;
        }
    }
}