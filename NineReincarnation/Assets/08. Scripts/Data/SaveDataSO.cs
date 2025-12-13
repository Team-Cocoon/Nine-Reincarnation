using UnityEngine;

public enum GameState
{
    Stoty,
    Stage
}


[CreateAssetMenu(fileName = "SaveDataSO", menuName = "Scriptable Objects/SaveDataSO")]
public class SaveDataSO : ScriptableObject
{
    public GameState State;
    public int StageIndex;
    public int StageSubIndex;
    public int StoryIndex;
    public int StorySubIndex;
    public Vector3? CheckPoint;

    public void Init()
    {
        State = GameState.Stoty;
        StageIndex = 0;
        StageSubIndex = 0;
        StoryIndex = 0;
        StorySubIndex = 0;
        CheckPoint = null;
    }

    public void Save(GameProgressData data)
    {
        State = data.State;
        StageIndex = data.StageIndex;
        StageSubIndex = data.StageSubIndex;
        StoryIndex = data.StoryIndex;
        StorySubIndex = data.StorySubIndex;
        CheckPoint = null;
    }
}
