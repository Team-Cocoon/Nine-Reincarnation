using Manager;
using State.SceneState;

namespace State.SceneState
{
    public class TitleState : ISceneState
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
        public TitleState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.TitleScenePath;
        }

        public void Enter()
        {
            _currentSceneState = SceneState.Title;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}