using DG.Tweening;
using Manager;

namespace State.SceneState
{
    public class ClearState : ISceneState
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
        public ClearState(CoreSceneLoader sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.ClearScenePath;
            _stateType = SceneStateType.Clear;
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