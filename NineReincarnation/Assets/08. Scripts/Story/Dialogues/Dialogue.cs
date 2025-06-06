using System.Reflection;
using Febucci.UI;
using UnityEngine;

[System.Serializable]
public class Dialogue { 
    [Header("현재 말하는 캐릭터")]
    public string objectName;

    [Header("대사")]
    public string contexts;

    [Header("표정")]
    public string expression;

    [Header("애니메이션 이름")]
    public string animName;

    [Header("이벤트 함수")]
    public string eventName;

    [Header("이벤트 종료 여부")]
    public bool isEnd = false;
}

[System.Serializable]
public class DialogueEvent
{
    public string eventName; // 대사 이벤트 이름
    public Dialogue dialogue;
}
