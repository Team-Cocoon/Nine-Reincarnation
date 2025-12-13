using VContainer;
using VContainer.Unity;

public class SceneTriggerInstaller : IInstaller
{
    private readonly SceneLoadManager _sceneLoadManager;
    private readonly FadeType _fadeType;

    public SceneTriggerInstaller(
        SceneLoadManager sceneLoadManager,
        FadeType fadeType
        )
    {
        _sceneLoadManager = sceneLoadManager;
        _fadeType = fadeType;
    }

    public void Install(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<SubSceneLoader>(Lifetime.Singleton)
            .WithParameter(_fadeType)
            .AsSelf();
        builder.Register<SceneTrigger>(Lifetime.Singleton);
        builder.RegisterComponent<SceneLoadManager>(_sceneLoadManager);
    }
}
