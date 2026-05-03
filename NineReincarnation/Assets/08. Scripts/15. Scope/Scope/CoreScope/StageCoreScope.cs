using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SceneLoadManager>(Lifetime.Singleton).As<SceneLoadManager>().AsImplementedInterfaces();

        builder.Register<StageInitiator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton).AsSelf();
    }
}
