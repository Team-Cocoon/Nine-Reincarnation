using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEventInterface
{
    public UniTask ExecuteEvent();
}

public class AsyncEvent : MonoBehaviour
{
    [SerializeField] private StoryEventManager _storyEventManager;
    private IEventInterface _event;

    private void Awake()
    {
        _event = GetComponent<IEventInterface>();
    }

    public void Execute()
    {
        RunTaskAsync().Forget();
    }

    private async UniTaskVoid RunTaskAsync()
    {
        if (_storyEventManager == null || _event == null)
        {
            _storyEventManager?.SignalEventComplete(); //즉시 완료시켜 멈춤 방지
            return;
        }

        await _event.ExecuteEvent();

        _storyEventManager.SignalEventComplete();
    }
}
