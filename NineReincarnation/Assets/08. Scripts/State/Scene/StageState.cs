using Manager;

namespace State.SceneState
{
    public class StageState : ISceneState
    {
        private SceneStateManager _sceneStateManager;

        private SceneState _currentSceneState;
        private string _scenePath;
        public SceneState CurrentSceneState => _currentSceneState;
        public string ScenePath => _scenePath;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="sceneStateManager"></param>
        public StageState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.StageScenePath;
        }

        public void Enter()
        {
            SceneEventHandler.SceneExited += SceneEvent_FadeOut;
            SceneEventHandler.SceneStarted += SceneEvent_FadeIn;
            _currentSceneState = SceneState.Stage;
        }

        public void Execute()
        {

        }

        public void Exit()
        {
            SceneEventHandler.SceneExited -= SceneEvent_FadeOut;
            SceneEventHandler.SceneStarted -= SceneEvent_FadeIn;
        }

        public void SceneEvent_FadeIn()
        {
            UIEventHandler.OnSceneFadeIn?.Invoke();
        }

        public void SceneEvent_FadeOut()
        {
            UIEventHandler.OnSceneFadeOut?.Invoke();
            SceneEvent_BgmPlay();
        }

        public void SceneEvent_BgmPlay()
        {
            //
        }
    }
}