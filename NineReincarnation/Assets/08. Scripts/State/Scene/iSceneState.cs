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
        public SceneState CurrentSceneState { get; }
    }
}