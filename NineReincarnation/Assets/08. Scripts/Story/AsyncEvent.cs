using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class AsyncEvent : MonoBehaviour
{
    [SerializeField] private List<int> _eventIdList;
    private IEventInterface _event;

    private Queue<int> _eventQueue;

    private void Awake()
    {
        _eventQueue = new Queue<int>(_eventIdList);
        _event = GetComponent<IEventInterface>();
    }

    public void Execute(StoryEventManager storyEventManager)
    {
        RunTaskAsync(storyEventManager).Forget();
    }

    private async UniTaskVoid RunTaskAsync(StoryEventManager storyEventManager)
    {
        if(_eventQueue.Count == 0)
        {
            await _event.ExecuteEvent(0);
        }
        else
        {
            await _event.ExecuteEvent(_eventQueue.Dequeue());
        }

        storyEventManager.SignalEventComplete();
    }
}
