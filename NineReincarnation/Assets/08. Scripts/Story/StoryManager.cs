using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }
    public Dictionary<string, IEvent> eventObj = new(); // 객체 매핑

    [Header("이벤트 호출 객체")]
    [SerializeField] private List<MonoBehaviour> _eventObjList; // 미리 이벤트 호출되는 객체들 담아둠
    [Header("현재 진행 중인 이벤트")]
    [SerializeField] private DialogueEvent _dialogueEvent;

    public DialogueEvent DialogueEvent => _dialogueEvent; 
    private void Awake()
    {
        Instance = this;
        MappingEventObject();
    }
    void OnDestroy()
    {
        eventObj.Clear();
    }

    public void NextDialogue()
    {
        _dialogueEvent.dialogue = DataManager.Instance.GetDialogue();
        _dialogueEvent.eventName = _dialogueEvent.dialogue.eventID;
    }

    /* 이벤트 호출하는 객체 저장 */
    private void MappingEventObject()
    {
        foreach (var obj in _eventObjList)
        {
            if (obj is IEvent handler)
            {
                var objName = handler.objName;
                eventObj[objName] = handler;
            }
        }
    }
    /// <summary>
    /// 현재 대화 정보 세팅
    /// </summary>
    public void SetDialogueData()
    {
        DialogueManager.Instance.SeTextData(_dialogueEvent.dialogue.contexts);
        DialogueManager.Instance.SetExpressionData(_dialogueEvent.dialogue.expression);
    }

    /// <summary>
    /// 애니메이션 실행
    /// </summary>
    public void StartAnim(Action eventAction = null)
    {
        if(eventAction != null)
        {
            eventObj[_dialogueEvent.dialogue.objectName]?.SettingAnimEvent(eventAction);
        }
        eventObj[_dialogueEvent.dialogue.objectName]?.StartAnim(_dialogueEvent.dialogue.animName);
    }
}
