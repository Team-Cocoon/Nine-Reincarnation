using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SubScene
{
    public List<string> SubScenePaths;
}

[Serializable]
public struct StateScene
{
    public string CoreScene;
    public List<SubScene> SubSceneGroups;
}

[CreateAssetMenu(fileName = "SceneDataSO", menuName = "Scriptable Objects/SceneDataSO")]
public class SceneDataSO : ScriptableObject
{
    public string LoadingScene;

    public string TitleScene;

    public string ClearScene;

    public StateScene StoryScene;

    public StateScene StageScene;
}
