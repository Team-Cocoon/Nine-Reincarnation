using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataSO", menuName = "Scriptable Objects/SaveDataSO")]
public class SaveDataSO : ScriptableObject
{
    public int StageIndex;
    public int StageSubIndex;
    public int StoryIndex;
    public int StorySubIndex;
    public Vector3? CheckPoint;

    public void Init()
    {
        StageIndex = 0;
        StageSubIndex = 1;
        StoryIndex = 0;
        StorySubIndex = 0;
        CheckPoint = null;
    }

    public void Save(GameProgressData data)
    {
        StageIndex = data.StageIndex;
        StageSubIndex = data.StageSubIndex;
        StoryIndex = data.StoryIndex;
        StorySubIndex = data.StorySubIndex;
        CheckPoint = null;
    }
}
