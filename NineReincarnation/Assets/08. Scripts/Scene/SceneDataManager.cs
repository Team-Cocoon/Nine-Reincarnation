using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance { get; private set; }

    [SerializeField] private SceneDataSO sceneData;

    public string LoadingScene => sceneData.LoadingScene;
    public string TitleScene => sceneData.TitleScene;
    public string StoryCoreScene => sceneData.StoryScene.CoreScene;
    public string StageCoreScene => sceneData.StageScene.CoreScene;
    public string ClearScene => sceneData.ClearScene;

    public List<string> GetStorySubScenes(int index)
    {
        return sceneData.StoryScene.SubSceneGroups[index].SubScenePaths;
    }

    public List<string> GetStageSubScenes(int index)
    {
        return sceneData.StageScene.SubSceneGroups[index].SubScenePaths;
    }

    public string GetStageSubScene(int index, int subIndex)
    {
        return sceneData.StageScene.SubSceneGroups[index].SubScenePaths[subIndex];
    }

    public string GetStorySubScene(int index, int subIndex)
    {
        return sceneData.StoryScene.SubSceneGroups[index].SubScenePaths[subIndex];
    }

    private void Awake()
    {
        Instance = this;
    }
}
