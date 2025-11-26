namespace State.SceneState
{
    public enum SceneStateType
    {
        Title,
        Stage,
        Story,
        Clear
    }

    public interface ISceneState : IState
    {
        public string ScenePath { get; }
        public SceneStateType StateType { get; }
    }
}