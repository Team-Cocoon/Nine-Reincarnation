public static class UIEventHandler
{
    public static event System.Action<int> OnMaxBlueThreadChanged;
    public static void OnMaxBlueThreadChanged_Invoke(int maxCount)
        => OnMaxBlueThreadChanged?.Invoke(maxCount);

    public static event System.Action<int> OnBlueThreadCountChanged;
    public static void OnBlueThreadCountChanged_Invoke(int currentCount)
        => OnBlueThreadCountChanged?.Invoke(currentCount);

    public static event System.Action<bool> OnBlueThreadConnectionChanged;
    public static void OnBlueThreadConnectionChanged_Invoke(bool isConnected)
        => OnBlueThreadConnectionChanged?.Invoke(isConnected);

    // ===== 홍연(빨간 실) =====
    // 연결됨
    public static event System.Action OnRedThreadConnected;
    public static void OnRedThreadConnected_Invoke()
        => OnRedThreadConnected?.Invoke();

    // 연결 대상과의 거리 비율(0 = 가까움/여유 많음, 1 = 끊기 직전)
    public static event System.Action<float> OnRedThreadDistanceChanged;
    public static void OnRedThreadDistanceChanged_Invoke(float ratio)
        => OnRedThreadDistanceChanged?.Invoke(ratio);

    // 연결 해제됨
    public static event System.Action OnRedThreadDisconnected;
    public static void OnRedThreadDisconnected_Invoke()
        => OnRedThreadDisconnected?.Invoke();

    // ===== 스토리 진행 =====
    // true = 스토리(다이얼로그) 진행 중 → HUD 숨김, false = 종료 → HUD 표시
    public static bool IsStoryPlaying { get; private set; }
    public static event System.Action<bool> OnStoryPlayingChanged;
    public static void OnStoryPlayingChanged_Invoke(bool playing)
    {
        IsStoryPlaying = playing;
        OnStoryPlayingChanged?.Invoke(playing);
    }
}