using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    [SerializeField] private SceneDataSO _sceneData;

    public string LoadingScene => _sceneData.LoadingScene;
    public string TitleScene => _sceneData.TitleScene;
    public string StageCoreScene => _sceneData.StageScene.CoreScene;
    public string ClearScene => _sceneData.ClearScene;

    public List<string> GetStageSubScenes(int index)
    {
        return _sceneData.StageScene.SubSceneGroups[index].SubScenePaths;
    }

    public string GetStageSubScene(int index, int subIndex)
    {
        return _sceneData.StageScene.SubSceneGroups[index].SubScenePaths[subIndex];
    }

    public bool HasStage(int index)
    {
        if (_sceneData.StageScene.Size <= index) return false;

        return true;
    }

    public bool NextStage(ref int index, ref int subIndex)
    {
        subIndex++;

        if (_sceneData.StageScene.SubSceneGroups[index].Size <= subIndex)
        {
            subIndex = 1;
            index++;

            return true;
        }
        return false;
    }
}
