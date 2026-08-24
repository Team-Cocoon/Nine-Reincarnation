using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AsyncEventData
{
    public int ID;
    public AsyncEvent StoryEvent;
}

public class StoryEventManager : MonoBehaviour
{
    [SerializeField] private AsyncEventData[] _asyncEventArray;

    private Dictionary<int, AsyncEvent> _asyncEventDict = new Dictionary<int, AsyncEvent>();

    private UniTaskCompletionSource _eventCompletionSource;
    private int curIndex = 0;

    private void Awake()
    {
        foreach (AsyncEventData data in _asyncEventArray)
        {
            _asyncEventDict.Add(data.ID, data.StoryEvent);
        }
    }

    private void OnDestroy()
    {
        SignalEventComplete();
    }

    public async UniTask ExcuteEvent(CancellationTokenSource cts, int id)
    {
        if (curIndex >= _asyncEventArray.Length) return;

        //완료 소스를 생성합니다.
        _eventCompletionSource = new UniTaskCompletionSource();

        // UnityEvent를 실행
        _asyncEventDict[id].Execute(this);

        // _eventCompletionSource.TrySetResult()가 호출될 때까지 여기서 대기
        await _eventCompletionSource.Task;
    }

    public void FinishEvent(int id)
    {
        if (curIndex >= _asyncEventArray.Length) return;
        _asyncEventDict[id].FinishExecute();
    }

    public void SignalEventComplete()
    {
        // 대기 중인 ExcuteEvent의 await를 해제
        _eventCompletionSource?.TrySetResult();
    }
}
