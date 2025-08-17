using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIEventHandler
{
    public static Action ToggleSettingUI;

    public static Func<Tween> OnSceneWipeFadeIn;

    public static Func<Tween> OnSceneFadeIn;

    public static Func<Tween> OnSceneWipeFadeOut;

    public static Func<Tween> OnSceneFadeOut;

    public static Action OnOpenListSeclectUI;

    public static Action OnOpenListUI;

    public static Action OnOpenInfoUI;

    public static Action OnOpenProfileUI;

    public static Action OnCloseMainUI;

    public static Action<Action> OnOpenListUpdateToolTipUI;

    public static Action OnOpenLockedListToolTipUI;

    public static Action OnOpenLockedProfileToolTipUI;
}
