using System;
using UnityEngine;

public static class InputEventHandler
{
    public static event Action OnChangedActionToUI;
    public static event Action OnChangedActionToPlayer;
    public static event Action OnChangedForceActionToUI;
    public static event Action OnChangedForceActionToPlayer;

    public static void OnChangedActionToUI_Invoke()          => OnChangedActionToUI?.Invoke();
    public static void OnChangedActionToPlayer_Invoke()      => OnChangedActionToPlayer?.Invoke();
    public static void OnChangedForceActionToUI_Invoke()     => OnChangedForceActionToUI?.Invoke();
    public static void OnChangedForceActionToPlayer_Invoke() => OnChangedForceActionToPlayer?.Invoke();
}
