using DG.Tweening;
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
            SceneEventHandler.SceneStarted += SceneEvent_BgmPlay;
            SceneEventHandler.SceneFadeOut += SceneEvent_FadeOut;
            SceneEventHandler.SceneFadeIn += SceneEvent_FadeIn;
            _currentSceneState = SceneState.Clear;
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
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.None);
        }
    }
}