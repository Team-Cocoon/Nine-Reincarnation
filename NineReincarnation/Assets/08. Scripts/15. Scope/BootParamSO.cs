using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "BootParamSO", menuName = "Scriptable Objects/BootParamSO")]
public class BootParamSO : ScriptableObject
{
    // 리다이렉트가 발생했는지 여부
    public bool IsTestMode { get; private set; }
    
    public SceneStateType SceneType { get; private set; }

    // 원래 실행하려고 했던 타겟 씬의 이름 (또는 인덱스)
    public string TargetSceneName { get; private set; }

    public int Chapter {get; private set;} = 0;
    public int Stage {get; private set;} = 0;

    /// <summary>
    /// 리다이렉트(테스트 진입) 타겟 설정
    /// </summary>
    /// <param name="sceneType"></param>
    /// <param name="sceneName"></param>
    public void SetPendingTarget(SceneStateType newSceneType, string sceneName)
    {
        IsTestMode = true;
        TargetSceneName = sceneName;
        SceneType = newSceneType;

        if (newSceneType == SceneStateType.Stage)
        {
            //TryParseStageFromSceneName(targetSceneName);
        }
    }

    public void Clear()
    {
        IsTestMode = false;
        SceneType = SceneStateType.None;
        TargetSceneName = null;
        Chapter = 0;
        Stage = 0;

    }

  private void TryParseStageFromSceneName(string sceneName)
    {
        // 예: Stage1-4, stage1-4
        var match = Regex.Match(sceneName, @"[Ss]tage(\d+)-(\d+)");

        if (!match.Success)
        {
            Debug.LogWarning($"[BootParamSO] Failed to parse stage from scene name: {sceneName}");
            return;
        }

        Chapter = int.Parse(match.Groups[1].Value);
        Stage = int.Parse(match.Groups[2].Value);
    }
}
 