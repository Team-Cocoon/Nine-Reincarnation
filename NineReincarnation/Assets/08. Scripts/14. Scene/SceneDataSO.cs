using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SubScene
{
    public List<string> SubScenePaths;
    public int Size => SubScenePaths.Count;
}

[Serializable]
public struct StateScene
{
    public string CoreScene;
    public List<SubScene> SubSceneGroups;
    public int Size => SubSceneGroups.Count;
}

[CreateAssetMenu(fileName = "SceneDataSO", menuName = "Scriptable Objects/SceneDataSO")]
public class SceneDataSO : ScriptableObject
{
    public string LoadingScene;

    public string TitleScene;

    public string ClearScene;

    public StateScene StageScene;
}
