using UnityEngine;
using VContainer;
using VContainer.Unity;

public class IntroScope : LifetimeScope
{
    [SerializeField] private NextScene _nextScene;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_nextScene);
    }
}
