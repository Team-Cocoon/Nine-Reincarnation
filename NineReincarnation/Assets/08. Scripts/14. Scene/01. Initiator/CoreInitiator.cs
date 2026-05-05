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
                    // 현재 reqPath가 해당 스테이지의 몇 번째 맵인지 찾습니다.
                    int mapIndex = _sceneDataManager.GetMapIndexByPath(stageIndex, reqPath);
                    
                    // 만약 맵 리스트에 없는 씬(예: 부트 씬 자체를 켜고 플레이 했을 때)이라면 mapIndex는 -1이 됩니다.
                    if (mapIndex != -1)
                    {
                        // 🌟 이미 잘 짜놓은 GetTargetScenes를 호출하여 리스트를 통째로 가져옵니다.
                        scenesToLoad = _sceneDataManager.GetTargetScenes(stageIndex, mapIndex);
                    }
                    else
                    {
                        // 예외 케이스: 맵이 아닌 다른 씬(부트씬 등)에서 플레이를 눌렀을 때
                        scenesToLoad.Add(_sceneDataManager.StageCoreScene);
                        scenesToLoad.Add(reqPath);
                    }
                }
                else // 스테이지 맵도 아니고 타이틀도 아닌 기타 씬인 경우
                {
                    scenesToLoad.Add(reqPath);
                }
            }

            await _transitionManager.TransitionToScenes(scenesToLoad, token);
            return;
        }
#endif

        // 빌드 버전이거나 에디터 코어 모드가 꺼져있을 땐 정상적으로 타이틀 로드
        scenesToLoad.Add(_sceneDataManager.TitleScene);
        await _transitionManager.TransitionToScenes(scenesToLoad, token);
    }
}