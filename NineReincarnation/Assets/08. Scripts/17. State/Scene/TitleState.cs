namespace State.SceneState
{
    public class TitleState : ISceneState
    {
        private CoreSceneLoader _sceneStateManager;

        private SceneStateType _stateType;
        private string _scenePath;
        public SceneStateType StateType => _stateType;
        public string ScenePath => _scenePath;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="sceneStateManager"></param>
        public TitleState(CoreSceneLoader sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.TitleScenePath;
            _stateType = SceneStateType.Title;
        }

        public void Enter()
        {

        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}