using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    [SerializeField] private SceneDataSO _sceneData;

    // 기존 프로퍼티들 완벽 유지!
    public string LoadingScene => _sceneData.LoadingScene;
    public string TitleScene => _sceneData.TitleScene;
    public string StageCoreScene => _sceneData.StageScene.CoreScene;
    public string ClearScene => _sceneData.ClearScene;

    // 🌟 [추가됨] 코어 + 부트 + 맵(있다면) 3개의 씬을 한 번에 리스트로 묶어주는 함수
    public List<string> GetTargetScenes(int index, int subIndex)
    {
        List<string> scenes = new List<string>();

        // 1. 코어씬
        if (!string.IsNullOrEmpty(StageCoreScene)) scenes.Add(StageCoreScene);

        if (HasStage(index))
        {
            var currentGroup = _sceneData.StageScene.SubSceneGroups[index];

            // 2. 부트씬 (값이 있다면)
            if (!string.IsNullOrEmpty(currentGroup.SubBootScene))
            {
                scenes.Add(currentGroup.SubBootScene);
            }

            // 3. 서브씬 (맵이 존재하고, 인덱스가 범위 안일 때만)
            if (currentGroup.Size > 0 && subIndex < currentGroup.Size)
            {
                scenes.Add(currentGroup.SubScenePaths[subIndex]);
            }
        }
        return scenes;
    }

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
        return _sceneData.StageScene.Size > index;
    }

    // 🌟 [수정됨] 스토리 전용(Size == 0)일 때의 예외 처리 추가
    public bool NextStage(ref int index, ref int subIndex)
    {
        subIndex++;

        var currentGroup = _sceneData.StageScene.SubSceneGroups[index];

        // 맵이 아예 없거나(스토리 씬), 맵의 끝에 도달했다면 다음 스테이지로!
        if (currentGroup.Size == 0 || currentGroup.Size <= subIndex)
        {
            // [참고] 기존 코드에서 subIndex = 1로 하셨는데, 
            // 리스트의 첫 번째 맵은 인덱스 0이므로 0으로 초기화하는 것이 맞을 것 같습니다!
            // (만약 1부터 시작해야 하는 특별한 이유가 있으시다면 1로 돌려주세요)
            subIndex = 0; 
            index++;

            return true; // 스테이지가 바뀌었음을 의미 (True)
        }
        return false; // 스테이지는 그대로고 맵만 바뀌었음을 의미 (False)
    }

    public int GetStageIndexByPath(string scenePath)
    {
        if (scenePath == StageCoreScene) return 0;

        for (int i = 0; i < _sceneData.StageScene.Size; i++)
        {
            // 🌟 [추가됨] 부트씬 경로도 체크하도록 추가
            if (_sceneData.StageScene.SubSceneGroups[i].SubBootScene == scenePath) return i;

            if (GetStageSubScenes(i).Contains(scenePath))
            {
                return i;
            }
        }
        return -1; 
    }

    public string GetSubBootScene(int stageIndex)
    {
        if (HasStage(stageIndex))
        {
            return _sceneData.StageScene.SubSceneGroups[stageIndex].SubBootScene;
        }
        return null;
    }

    // [추가 팁] 맵 인덱스도 경로로 찾을 수 있으면 편리합니다.
    public int GetMapIndexByPath(int stageIndex, string scenePath)
    {
        if (!HasStage(stageIndex)) return -1;
        return _sceneData.StageScene.SubSceneGroups[stageIndex].SubScenePaths.IndexOf(scenePath);
    }
}