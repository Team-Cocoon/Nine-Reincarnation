using Manager;
using State.SceneState;
using Utilities;

namespace State.SceneState
{
    public class StoryState : ISceneState
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
        public StoryState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.StoryScenePath;
        }

        public void Enter()
        {
            _currentSceneState = SceneState.Story;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}