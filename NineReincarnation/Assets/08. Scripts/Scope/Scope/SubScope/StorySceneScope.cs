using Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StorySceneScope : LifetimeScope
{
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private LoadNextScene _loadNextScene;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<CameraManager>(_cameraManager);

        if(_loadNextScene != null)
        {
            builder.RegisterComponent<LoadNextScene>(_loadNextScene);
        }
    }
}
