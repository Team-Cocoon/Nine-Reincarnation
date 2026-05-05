using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

public class EntryPoint : IInitializable, IAsyncStartable, IDisposable
{
    [Inject] private IInitiator _initiator;

    [Inject] private TransitionTaskRegistry _taskRegistry; // 🌟 공용 등록소 주입

    private CancellationTokenSource _cts;
    private UniTaskCompletionSource _tcs; // 🌟 작업 완료를 알릴 신호탄

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
        _tcs = new UniTaskCompletionSource();

        // 🌟 유니티 씬 로딩 직후(Awake 타이밍), 매니저에게 "내 작업 기다려!" 라고 대기표를 넘깁니다.
        // 이때 _tcs.Task는 아직 완료되지 않은 상태(대기 상태)의 UniTask입니다.
        if (_taskRegistry != null)
        {
            _taskRegistry.RegisterTask(_tcs.Task);
        }
    }

    public void Dispose()
    {
       if(_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public async UniTask StartAsync(CancellationToken cancellation = default)
    {
        try
        {
            // 실제 데이터 로드 및 맵 생성 등의 무거운 작업 실행
            await _initiator.GameInitialize(_cts.Token);
        }
        finally
        {
            // 이걸 호출하는 순간 매니저가 기다리던 WaitAllTasksAsync()가 풀리면서 로딩창이 닫힙니다!
            _tcs?.TrySetResult();
        }
    }
}
