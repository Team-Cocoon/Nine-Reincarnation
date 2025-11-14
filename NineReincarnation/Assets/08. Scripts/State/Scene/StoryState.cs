using DG.Tweening;
using Manager;

namespace State.SceneState
{
    public class StoryState : ISceneState
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
        public StoryState(CoreSceneLoader sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.StoryScenePath;
            _stateType = SceneStateType.Story;
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

        public Tween SceneEvent_FadeIn()
        {
            return UIEventHandler.OnSceneFadeIn_Invoke(true);
        }

        public Tween SceneEvent_FadeOut()
        {
            return UIEventHandler.OnSceneFadeOut_Invoke(true);
        }

        public void SceneEvent_BgmPlay()
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.Stage);
        }
    }
}