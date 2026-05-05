using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<StageManager>(Lifetime.Singleton);
    }
}
