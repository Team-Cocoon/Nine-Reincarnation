using State;
using State.SceneState;
using StateMachine.SceneStateMachine;
using UnityEngine;


namespace Manager
{
    public class SceneStateManager : MonoBehaviour
    {
        [Header("--- 핵심 씬 ---")]
        [SerializeField] private string _titleScenePath;
        [SerializeField] private string _storyScenePath;
        [SerializeField] private string _stageScenePath;
        [SerializeField] private string _clearScenePath;

        private SceneStateMachine _sceneStateMachine;
        private string _currentScenePath;

        public SceneStateMachine SceneStateMachine => _sceneStateMachine;
        public string TitleScenePath => _titleScenePath;
        public string StageScenePath => _stageScenePath;
        public string StoryScenePath => _storyScenePath;
        public string ClearScenePath => _clearScenePath;


        private void Awake()
        {
            _currentScenePath = "";
            _sceneStateMachine = new SceneStateMachine(this);
        }

        private void Start()
        {
            _sceneStateMachine.Initialize(_sceneStateMachine._titleState);
        }

        private void OnEnable()
        {
            _sceneStateMachine.stateChanged += ChangeScene;

            GameEventHandler.TitleExcuted += GameEvent_ToTitle;
            GameEventHandler.StoryExcuted += GameEvent_ToStory;
            GameEventHandler.StageExcuted += GameEvent_ToStage;
            GameEventHandler.GameClearExcuted += GameEvent_ToClear;
        }

        private void OnDisable()
        {
            _sceneStateMachine.stateChanged += ChangeScene;

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

            Debug.Log(scenePath);

            SceneEventHandler.SceneStateChanged(scenePath, _currentScenePath);

            _currentScenePath = scenePath;
        }
    }
}