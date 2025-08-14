using UnityEngine;

public class StorySceneManager : MonoBehaviour
{
    [SerializeField] private string[] storyScenePaths;

    private void Awake()
    {
        int index = SaveManager.Instance.SaveData.StoryIndex;
        SceneEventHandler.SceneLoadedByPath(storyScenePaths[index]);
    }
}
