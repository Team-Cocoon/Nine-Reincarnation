using DG.Tweening;
using Manager;

namespace State.SceneState
{
    public class ClearState : ISceneState
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
        public ClearState(SceneStateManager sceneStateManager)
        {
            _sceneStateManager = sceneStateManager;
            _scenePath = sceneStateManager.ClearScenePath;
            _stateType = SceneState.Clear;
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
            return UIEventHandler.OnSceneFadeIn_Invoke(true);
        }

        public Tween SceneEvent_FadeOut()
        {
            return UIEventHandler.OnSceneFadeOut_Invoke(true);
        }

        public void SceneEvent_BgmPlay()
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.None);
        }
    }
}