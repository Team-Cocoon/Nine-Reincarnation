using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SceneLoadScope : LifetimeScope
{
    [SerializeField] private SceneLoadManager _sceneLoadManager;
    [SerializeField] private SubSceneLoader _subSceneLoader;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<SceneLoadManager>(_sceneLoadManager);
        builder.RegisterComponent<SubSceneLoader>(_subSceneLoader);
    }
}
