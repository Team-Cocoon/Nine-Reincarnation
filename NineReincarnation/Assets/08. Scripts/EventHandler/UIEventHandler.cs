using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIEventHandler
{
    public static Action ToggleSettingUI;

    public static Func<bool,Tween> OnSceneWipeFadeIn;

    public static Func<bool, Tween> OnSceneFadeIn;

    public static Func<bool, Tween> OnSceneWipeFadeOut;

    public static Func<bool, Tween> OnSceneFadeOut;

    public static Action OnOpenListSeclectUI;

    public static Action OnOpenListUI;

    public static Action OnOpenInfoUI;

    public static Action OnOpenProfileUI;

    public static Action OnCloseMainUI;

    public static Action<Action> OnOpenListUpdateToolTipUI;

    public static Action OnOpenLockedListToolTipUI;

    public static Action OnOpenLockedProfileToolTipUI;
}
