using System;

public class SceneEventHandler
{
    public static Action<Action> OnSceneStart;

    public static Action<string> SceneLoadedByPath;

    public static Action<string, string> SceneStateChanged;

    public static Action<string> SceneLoadedAdditivelyByPath;

    public static Action<string> SceneUnloadedByPath;

    public static Action AllSceneUnloaded;

    public static Action LastSceneUnloaded;
}
