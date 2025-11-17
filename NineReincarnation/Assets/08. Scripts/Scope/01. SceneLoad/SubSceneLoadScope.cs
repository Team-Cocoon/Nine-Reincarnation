using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SubSceneLoadScope : LifetimeScope
{
    [SerializeField] private LoadNextScene _loadnextScene;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<LoadNextScene>(_loadnextScene);
    }
}
