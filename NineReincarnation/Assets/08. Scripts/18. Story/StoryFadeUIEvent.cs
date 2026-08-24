using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryFadeUIEvent : MonoBehaviour, IEventInterface
{
    [SerializeField] StoryFadeUI _storyUI;

    public enum EventType
    {
        FadeIn = 0,
        FadeOut = 1,
        FadeInOut = 2,
    }

    [Serializable]
    public struct FadeCanvasEvent
    {
        public EventType EventType;
        public float StartUpTime;
        public float EventDuration;
    }

    [Header("Should be matching with AsyncEvent List Index")]
    [SerializeField] private List<FadeCanvasEvent> _eventList;

    public UniTask ExecuteEvent(int index)
    {
        if(_eventList == null || index < 0 || index >= _eventList.Count)
        {
            return UniTask.CompletedTask;
        }

        return CanvasEvent(_eventList[index]);
    }

    public void FinishEvent(int index)
    {
        switch (_eventList[index].EventType)
        {
            case EventType.FadeIn:
                _storyUI.FadeIn();
                break;
            case EventType.FadeOut:
            case EventType.FadeInOut:
                _storyUI.FadeOut();
                break;
        }
    }

    private async UniTask CanvasEvent(FadeCanvasEvent _eventInfo)
    {
        switch (_eventInfo.EventType)
        {
            case EventType.FadeIn: 
                await UniTask.WaitForSeconds(_eventInfo.StartUpTime);
                await _storyUI.FadeIn(_eventInfo.EventDuration); 
                break;
            case EventType.FadeOut:
                await UniTask.WaitForSeconds(_eventInfo.StartUpTime);
                await _storyUI.FadeOut(_eventInfo.EventDuration); 
                break;
            case EventType.FadeInOut:
                await _storyUI.FadeInOut(_eventInfo.EventDuration, _eventInfo.StartUpTime);
                break;
        }
    }

}
