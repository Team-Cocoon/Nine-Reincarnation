using DG.Tweening;

namespace State.SceneState
{
    public enum SceneState
    {
        Title,
        Stage,
        Story,
        Clear
    }

    public interface ISceneState : IState
    {
        public string ScenePath { get; }
        public SceneState StateType { get; }

        public Tween SceneEvent_FadeIn();

        public Tween SceneEvent_FadeOut();

        public void SceneEvent_BgmPlay();
    }
}