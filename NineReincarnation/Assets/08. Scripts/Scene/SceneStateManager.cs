using System.Collections.Generic;
using State;
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

            _sceneStateMachine.stateChanged += ChangeScene;

            _sceneStateMachine.Initialize(_sceneStateMachine._titleState);

            GameEventHandler.TitleExcuted += GameEvent_ToTitle;
            GameEventHandler.StoryExcuted += GameEvent_ToStory;
            GameEventHandler.StageExcuted += GameEvent_ToStage;
            GameEventHandler.GameClearExcuted += GameEvent_ToClear;
        }
        

        private void OnDestroy()
        {
            _sceneStateMachine.stateChanged -= ChangeScene;

            GameEventHandler.TitleExcuted -= GameEvent_ToTitle;
            GameEventHandler.StoryExcuted -= GameEvent_ToStory;
            GameEventHandler.StageExcuted -= GameEvent_ToStage;
            GameEventHandler.GameClearExcuted -= GameEvent_ToClear;
        }

        private void GameEvent_ToTitle()
        {
            GameEvent_TransitionState(SceneState.Title);
        }

        private void GameEvent_ToStory()
        {
            GameEvent_TransitionState(SceneState.Story);
        }

        private void GameEvent_ToStage()
        {
            GameEvent_TransitionState(SceneState.Stage);
        }

        private void GameEvent_ToClear()
        {
            GameEvent_TransitionState(SceneState.Clear);
        }

        private void GameEvent_TransitionState(SceneState state)
        {
            _sceneStateMachine.TransitionState(state);
        }

        private void ChangeScene(IState state)
        {
            string scenePath = (state as ISceneState).ScenePath;

            switch ((state as ISceneState).CurrentSceneState)
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