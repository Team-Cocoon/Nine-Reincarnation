using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

public class CoreInitiator : IInitiator
{
    private readonly SceneTransitionManager _transitionManager;
    private readonly SceneDataManager _sceneDataManager;

    [Inject]
    public CoreInitiator(SceneTransitionManager transitionManager, SceneDataManager sceneDataManager)
    {
        _transitionManager = transitionManager;
        _sceneDataManager = sceneDataManager;
    }

    public async UniTask GameInitialize(CancellationToken token)
    {
        List<string> scenesToLoad = new List<string>();

#if UNITY_EDITOR
        string reqPath = CoreBootStrap.RequestedStartScenePath;
        string reqName = CoreBootStrap.RequestedStartSceneName;

        if (!string.IsNullOrEmpty(reqPath) && !reqName.Contains("BaseScene") && ToolbarPlayButtonsView.OnGetCoreMode)
        {
            if (reqPath == _sceneDataManager.TitleScene)
            {
                scenesToLoad.Add(_sceneDataManager.TitleScene);
            }
            else 
            {
                int stageIndex = _sceneDataManager.GetStageIndexByPath(reqPath);
                if (stageIndex != -1)
                {
                    scenesToLoad.Add(_sceneDataManager.StageCoreScene);
                    scenesToLoad.AddRange(_sceneDataManager.GetStageSubScenes(stageIndex));
                }
                else
                {
                    scenesToLoad.Add(reqPath);
                }
            }

            await _transitionManager.TransitionToScenes(scenesToLoad, token);
            return;
        }
#endif

        scenesToLoad.Add(_sceneDataManager.TitleScene);
        await _transitionManager.TransitionToScenes(scenesToLoad, token);
    }
}