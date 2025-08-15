using DG.Tweening;
using Manager;
using State.SceneState;
using Utilities;

namespace State.SceneState
{
    public class StoryState : ISceneState
    {
        private SceneStateManager _sceneStateManager;

        private SceneState _stateType;
        private string _scenePath;
        public SceneState StateType => _stateType;
        public string ScenePath => _scenePath;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="sceneStateManager"></param>
        public StoryState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.StoryScenePath;
            _stateType = SceneState.Story;
        }

        public void Enter()
        {
            SceneEventHandler.SceneStarted += SceneEvent_BgmPlay;
            SceneEventHandler.SceneFadeOut += SceneEvent_FadeOut;
            SceneEventHandler.SceneFadeIn += SceneEvent_FadeIn;
        }

        public void Execute()
        {

        }

        public void Exit()
        {
            SceneEventHandler.SceneStarted -= SceneEvent_BgmPlay;
            SceneEventHandler.SceneFadeOut -= SceneEvent_FadeOut;
            SceneEventHandler.SceneFadeIn -= SceneEvent_FadeIn;
        }

        public Tween SceneEvent_FadeIn()
        {
            return UIEventHandler.OnSceneFadeIn_Invoke();
        }

        public Tween SceneEvent_FadeOut()
        {
            return UIEventHandler.OnSceneFadeOut_Invoke();
        }

        public void SceneEvent_BgmPlay()
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.Stage);
        }
    }
}