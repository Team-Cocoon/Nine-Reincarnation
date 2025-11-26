namespace State.SceneState
{
    public class StageState : ISceneState
    {
        private CoreSceneLoader _sceneStateManager;
        private AudioManager _audioManager;

        private SceneStateType _stateType;
        private string _scenePath;
        public SceneStateType StateType => _stateType;
        public string ScenePath => _scenePath;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="sceneStateManager"></param>
        public StageState(CoreSceneLoader sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.StageScenePath;
            _stateType = SceneStateType.Stage;
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