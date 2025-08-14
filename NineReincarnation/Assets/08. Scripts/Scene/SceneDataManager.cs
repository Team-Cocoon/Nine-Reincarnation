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
    public string ClearScene => sceneData.StageScene.CoreScene;

    public List<string> GetStorySubScene(int index)
    {
        return sceneData.StoryScene.SubSceneGroups[index].SubScenePaths;
    }

    public List<string> GetStageSubScene(int index)
    {
        return sceneData.StageScene.SubSceneGroups[index].SubScenePaths;
    }

    private void Awake()
    {
        Instance = this;
    }
}
