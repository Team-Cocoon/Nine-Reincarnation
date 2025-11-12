using System;
using System.Collections.Generic;
using DG.Tweening;

public static class SceneEventHandler
{
    public static event Action<string> OnSceneChanged;

    public static event Action SceneExited;

    public static event Func<Tween> SceneFadeOut;

    public static event Action SceneStarted;

    public static event Func<Tween> SceneFadeIn;

    public static event Action AllSceneUnloaded;

    public static event Action LastSceneUnloaded;


    #region Invoke 처리
    public static void OnSceneChanged_Invoke(string scenePath) => OnSceneChanged?.Invoke(scenePath);
    public static void SceneExited_Invoke() => SceneExited?.Invoke();
    public static Tween SceneFadeOut_Invoke()
    {
        return SceneFadeOut?.Invoke();
    }
    public static void SceneStarted_Invoke() => SceneStarted?.Invoke();
    public static Tween SceneFadeIn_Invoke()
    {
        return SceneFadeIn?.Invoke();
    }
    public static void AllSceneUnloaded_Invoke() => AllSceneUnloaded?.Invoke();
    public static void LastSceneUnloaded_Invoke() => LastSceneUnloaded?.Invoke();
    #endregion
}
