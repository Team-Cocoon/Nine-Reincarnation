using System;
using System.Collections.Generic;
using DG.Tweening;
using State.SceneState;
using UnityEngine;

public static class UIEventHandler
{
    public static event Action ToggleSettingUI;

    public static event Func<Tween> OnSceneWipeFadeIn;

    public static event Func<Tween> OnSceneFadeIn;

    public static event Func<Tween> OnSceneWipeFadeOut;

    public static event Func<Tween> OnSceneFadeOut;

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
    public static Tween OnSceneWipeFadeIn_Invoke()
    {
        return OnSceneWipeFadeIn?.Invoke();
    }
    public static Tween OnSceneFadeIn_Invoke()
    {
        return OnSceneFadeIn?.Invoke();
    }
    public static Tween OnSceneWipeFadeOut_Invoke()
    {
        return OnSceneWipeFadeOut?.Invoke();
    }
    public static Tween OnSceneFadeOut_Invoke()
    {
        return OnSceneFadeOut?.Invoke();
    }
    #endregion
}
