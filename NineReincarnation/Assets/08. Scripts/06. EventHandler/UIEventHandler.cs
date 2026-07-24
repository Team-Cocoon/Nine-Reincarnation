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
}