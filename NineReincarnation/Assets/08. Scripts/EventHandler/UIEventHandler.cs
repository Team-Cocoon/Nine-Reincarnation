using System;
using DG.Tweening;

public static class UIEventHandler
{
    public static event Action<bool> ToggleSettingUI;

    public static Func<bool, Tween> OnSceneWipeFadeIn;

    public static Func<bool, Tween> OnSceneFadeIn;

    public static Func<bool, Tween> OnSceneWipeFadeOut;

    public static Func<bool, Tween> OnSceneFadeOut;

    public static event Action OnOpenListSeclectUI;

    public static event Action OnOpenListUI;

    public static event Action OnOpenInfoUI;

    public static event Action OnOpenProfileUI;

    public static event Action OnCloseMainUI;

    public static event Action<Action> OnOpenListUpdateToolTipUI;

    public static event Action OnOpenLockedListToolTipUI;

    public static event Action OnOpenLockedProfileToolTipUI;

    #region Invoke 처리
    public static void ToggleSettingUI_Invoke(bool isStop) => ToggleSettingUI?.Invoke(isStop);
    public static Tween OnSceneWipeFadeIn_Invoke(bool stopTime)
    {
        return OnSceneWipeFadeIn?.Invoke(stopTime);
    }
    public static Tween OnSceneFadeIn_Invoke(bool stopTime)
    {
        return OnSceneFadeIn?.Invoke(stopTime);
    }
    public static Tween OnSceneWipeFadeOut_Invoke(bool stopTime)
    {
        return OnSceneWipeFadeOut?.Invoke(stopTime);
    }
    public static Tween OnSceneFadeOut_Invoke(bool stopTime)
    {
        return OnSceneFadeOut?.Invoke(stopTime);
    }
    #endregion
}
