using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIEventHandler
{
    public static bool isOpen = false;

    public static Action OnOpenListSeclectUI;

    public static Action OnOpenListUI;

    public static Action OnOpenInfoUI;

    public static Action OnOpenProfileUI;

    public static Action OnCloseMainUI;

    public static Action OnOpenListUpdateToolTipUI;

    public static Action OnOpenLockedListToolTipUI;

    public static Action OnOpenLockedProfileToolTipUI;
    public static bool ToggleMainUI()
    {
        if (isOpen)
        {
            Time.timeScale = 1.0f;
            OnCloseMainUI();
            isOpen = false;         
        }
        else
        {
            Time.timeScale = 0.0f;
            OnOpenListSeclectUI();
            isOpen = true;
        }

        return isOpen;
    }
}
