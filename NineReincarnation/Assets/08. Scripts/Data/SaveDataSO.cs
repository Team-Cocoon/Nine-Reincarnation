using UnityEngine;

public enum GameState
{ 
    Stoty,
    Stage
}


[CreateAssetMenu(fileName = "SaveDataSo", menuName = "Scriptable Objects/SaveDataSo")]
public class SaveDataSO : ScriptableObject
{
    public GameState State;
    public int StageIndex;
    public int StoryIndex;
    public Vector3? CheckPoint;

    public void Init()
    {
        State = GameState.Stoty;
        StageIndex = 0;
        StoryIndex = 0;
        CheckPoint = null;
    }
}
