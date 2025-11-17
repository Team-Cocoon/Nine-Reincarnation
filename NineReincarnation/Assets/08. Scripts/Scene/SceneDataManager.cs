using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    [SerializeField] private SceneDataSO _sceneData;

    public string LoadingScene => _sceneData.LoadingScene;
    public string TitleScene => _sceneData.TitleScene;
    public string StoryCoreScene => _sceneData.StoryScene.CoreScene;
    public string StageCoreScene => _sceneData.StageScene.CoreScene;
    public string ClearScene => _sceneData.ClearScene;

    public List<string> GetStorySubScenes(int index)
    {
        return _sceneData.StoryScene.SubSceneGroups[index].SubScenePaths;
    }

    public List<string> GetStageSubScenes(int index)
    {
        return _sceneData.StageScene.SubSceneGroups[index].SubScenePaths;
    }

    public string GetStageSubScene(int index, int subIndex)
    {
        return _sceneData.StageScene.SubSceneGroups[index].SubScenePaths[subIndex];
    }

    public string GetStorySubScene(int index, int subIndex)
    {
        return _sceneData.StoryScene.SubSceneGroups[index].SubScenePaths[subIndex];
    }

    public bool NextStage(ref int index, ref int subIndex)
    {
        Debug.Log(_sceneData.StageScene.SubSceneGroups[index].Size);
        Debug.Log(subIndex + 1);

        if (_sceneData.StageScene.SubSceneGroups[index].Size <= subIndex + 1) 
        {
            subIndex = 0;

            if (_sceneData.StageScene.Size <= index + 1)
            {
                return false;
            }

            index++;
            return true;
        }

        subIndex++;
        return true;
    }

    public bool NextStory(ref int index, ref int subIndex)
    {
        if (_sceneData.StoryScene.SubSceneGroups[index].Size <= subIndex + 1)
        {
            subIndex = 0;

            if (_sceneData.StoryScene.Size <= index + 1)
            {
                return false;
            }

            index++;
            return true;
        }

        subIndex++;
        return true;
    }
}
