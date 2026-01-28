using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    [Header("---- SceneTriggerInstaller ----")]
    [SerializeField] private SceneLoadManager _sceneLoadManager;
    [SerializeField] private LoadNextScene _loadNextScene;
    [SerializeField] private FadeType _fadeType;

    protected override void Configure(IContainerBuilder builder)
    {
        new SceneTriggerInstaller(_sceneLoadManager, _fadeType).Install(builder);
        builder.RegisterComponent(_loadNextScene);

    }

    private void Start()
    {

    }
}
