using Manager;
using Player.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageInitScope : LifetimeScope
{
    [Header("----- Stage Init -----")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private ThrowThread _thread;
    [SerializeField] private LoadNextScene _loadNextScene;
    [SerializeField] private CameraManager _cameraManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<PlayerController>(_player);
        builder.RegisterComponent<ThrowThread>(_thread);
        builder.RegisterComponent<LoadNextScene>(_loadNextScene);
        builder.RegisterComponent<CameraManager>(_cameraManager);
    }
}