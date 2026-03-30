using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataSO", menuName = "Scriptable Objects/SaveDataSO")]
public class SaveDataSO : ScriptableObject
{
    public int StageIndex;
    public int StageSubIndex;
    public SceneStateType State;
    public Vector3? CheckPoint;

    public void Init()
    {
        StageIndex = 0;
        StageSubIndex = 1;
        CheckPoint = null;
    }

    public void Save(GameProgressData data)
    {
        StageIndex = data.StageIndex;
        StageSubIndex = data.StageSubIndex;
        State = data.State;
        CheckPoint = null;
    }
}
