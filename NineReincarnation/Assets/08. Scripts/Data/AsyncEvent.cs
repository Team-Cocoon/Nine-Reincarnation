using Cysharp.Threading.Tasks;
using UnityEngine;
public class AsyncEvent : MonoBehaviour
{
    private IEventInterface _event;

    private void Awake()
    {
        _event = GetComponent<IEventInterface>();
    }

    public void Execute(StoryEventManager storyEventManager)
    {
        RunTaskAsync(storyEventManager).Forget();
    }

    private async UniTaskVoid RunTaskAsync(StoryEventManager storyEventManager)
    {
        await _event.ExecuteEvent();

        storyEventManager.SignalEventComplete();
    }
}
