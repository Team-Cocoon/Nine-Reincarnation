using System;
using DG.Tweening;

public static class UIEventHandler
{
#pragma warning disable UDR0001
    public static event Action ToggleSettingUI;
    public static Func<bool, Tween> OnSceneWipeFadeIn;
    public static Func<bool, Tween> OnSceneFadeIn;
    public static Func<bool, Tween> OnSceneWipeFadeOut;
    public static Func<bool, Tween> OnSceneFadeOut;
#pragma warning restore UDR0001

    public static event Action OnOpenListSeclectUI;
    public static event Action OnOpenListUI;
    public static event Action OnOpenInfoUI;
    public static event Action OnOpenProfileUI;
    public static event Action OnCloseMainUI;
    public static event Action<Action> OnOpenListUpdateToolTipUI;
    public static event Action OnOpenLockedListToolTipUI;
    public static event Action OnOpenLockedProfileToolTipUI;

    #region Invoke 처리
    public static void ToggleSettingUI_Invoke() => ToggleSettingUI?.Invoke();
    public static Tween OnSceneWipeFadeIn_Invoke(bool stopTime) => OnSceneWipeFadeIn?.Invoke(stopTime);
    public static Tween OnSceneFadeIn_Invoke(bool stopTime) => OnSceneFadeIn?.Invoke(stopTime);
    public static Tween OnSceneWipeFadeOut_Invoke(bool stopTime) => OnSceneWipeFadeOut?.Invoke(stopTime);
    public static Tween OnSceneFadeOut_Invoke(bool stopTime) => OnSceneFadeOut?.Invoke(stopTime);
    #endregion
}
