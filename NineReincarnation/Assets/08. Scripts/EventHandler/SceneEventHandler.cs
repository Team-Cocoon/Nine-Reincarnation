using System;
using DG.Tweening;

public class SceneEventHandler
{
    public static Action SceneExited;

    public static Func<Tween> SceneFadeOut;

    public static Action SceneStarted;

    public static Func<Tween> SceneFadeIn;

    public static Action<string> SceneLoadedByPath;

    public static Action<string, string> SceneStateChanged;

    public static Action<string> SceneLoadedAdditivelyByPath;

    public static Action<string> SceneUnloadedByPath;

    public static Action AllSceneUnloaded;

    public static Action LastSceneUnloaded;
}
