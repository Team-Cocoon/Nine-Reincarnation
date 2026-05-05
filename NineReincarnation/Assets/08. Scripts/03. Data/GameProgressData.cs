using UnityEngine;


public class GameProgressData
{
    //public GameState State;
    public int StageIndex;
    public int StageSubIndex;
    public SceneStateType State;
    public Vector3? CheckPoint;

    public GameProgressData(SaveDataSO saveData)
    {
        //State = saveData.State;
        StageIndex = saveData.StageIndex;
        StageSubIndex = saveData.StageSubIndex;
    }
}
