using Manager;

namespace State.SceneState
{
    public class ClearState : ISceneState
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
        public ClearState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.ClearScenePath;
        }

        public void Enter()
        {
            _currentSceneState = SceneState.Clear;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}