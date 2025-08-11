using System;
using UnityEngine;

public class UIEventHandler
{
    public static Action ToggleSettingUI;

    public static Action OnSceneWipeFadeIn;

    public static Action OnSceneFadeIn;

    public static Action OnSceneWipeFadeOut;

    public static Action OnSceneFadeOut;

    public static Action OnOpenListSeclectUI;

    public static Action OnOpenListUI;

    public static Action OnOpenInfoUI;

    public static Action OnOpenProfileUI;

    public static Action OnCloseMainUI;

    public static Action<Action> OnOpenListUpdateToolTipUI;

    public static Action OnOpenLockedListToolTipUI;

    public static Action OnOpenLockedProfileToolTipUI;
}
