using System;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent[] _storyEvent;
    private int curIndex = 0;

    public void ExcuteEvent()
    {
        if (curIndex >= _storyEvent.Length) return;
        _storyEvent[curIndex++]?.Invoke();
    }


}
