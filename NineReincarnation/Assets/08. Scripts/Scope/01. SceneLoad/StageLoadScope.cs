using Player.Controller;
using UnityEditor.Overlays;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageLoadScope : LifetimeScope
{
    [SerializeField] private SceneLoadManager _sceneLoadManager;
    [SerializeField] private SubSceneLoader _subSceneLoader;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<SceneLoadManager>(_sceneLoadManager);
        builder.RegisterComponent<SubSceneLoader>(_subSceneLoader);
    }

    private void Start()
    {
        AudioManager.Instance?.PlayBgm(AudioManager.Bgm.Stage);
    }
}
