using UnityEngine;
using VContainer;

public class StageSceneLoader : SceneLoader
{
    [Inject] private SceneLoadManager _loadManager;

    public async void LoadStageScene()
    {
        //string path = _loadManager.GetScenePath(SceneType.Stage);
        //await 
    }

}
