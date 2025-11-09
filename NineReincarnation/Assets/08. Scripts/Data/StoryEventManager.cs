using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class StoryEventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent[] _storyEvent;

    private UniTaskCompletionSource _eventCompletionSource;
    private int curIndex = 0;

    public async UniTask ExcuteEvent() // UniTaskVoid -> UniTask
    {
        if (curIndex >= _storyEvent.Length) return;

        //완료 소스를 생성합니다.
        _eventCompletionSource = new UniTaskCompletionSource();

        // UnityEvent를 실행
        _storyEvent[curIndex++].Invoke();

        // _eventCompletionSource.TrySetResult()가 호출될 때까지 여기서 대기
        await _eventCompletionSource.Task;
    }

    public void SignalEventComplete()
    {
        // 대기 중인 ExcuteEvent의 await를 해제
        _eventCompletionSource?.TrySetResult();
    }
}
