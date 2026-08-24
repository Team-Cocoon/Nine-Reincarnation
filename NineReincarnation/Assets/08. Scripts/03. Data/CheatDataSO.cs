using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheatInfo
{
    public int StageNumber;
    public int MovePointNumber;
    public int PointIndex;
    public int SubSceneNumber;

    public string OptionName => $"{StageNumber}-{MovePointNumber}";
}

[CreateAssetMenu(fileName = "CheatDataSO", menuName = "Scriptable Objects/CheatDataSO")]
public class CheatDataSO : ScriptableObject
{
    public List<CheatInfo> CheatInfoList;
}
