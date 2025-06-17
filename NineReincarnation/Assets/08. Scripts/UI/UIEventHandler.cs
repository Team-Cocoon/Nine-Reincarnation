using System;
using UnityEngine;

public class UIEventHandler
{
    public static bool isMainUIOpen = false;
    public static bool isSettingUIOpen = false;

    public static Action OnOpenListSeclectUI;

    public static Action OnOpenListUI;

    public static Action OnOpenInfoUI;

    public static Action OnOpenProfileUI;

    public static Action OnCloseMainUI;

    public static Action<Action> OnOpenListUpdateToolTipUI;

    public static Action OnOpenLockedListToolTipUI;

    public static Action OnOpenLockedProfileToolTipUI;

    public static Action<Action> OnSceneWipeFadeIn;

    public static Action<Action> OnSceneFadeIn;

    public static Action<Action> OnSceneWipeFadeOut;

    public static Action<Action> OnSceneFadeOut;

    public static Action OnOpenSettingUI;

    public static Action OnCloseSettingUI;


    public static bool ToggleMainUI()
    {
        if (isMainUIOpen)
        {
            Time.timeScale = 1.0f;
            OnCloseMainUI();
            isMainUIOpen = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            OnOpenListSeclectUI();
            isMainUIOpen = true;
        }

        return isMainUIOpen;
    }

    public static bool ToggleSettingUI()
    {
        if (isSettingUIOpen)
        {
            Time.timeScale = 1.0f;
            OnCloseSettingUI();
            isSettingUIOpen = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            OnOpenSettingUI();
            isSettingUIOpen = true;
        }

        return isSettingUIOpen;
    }
}
