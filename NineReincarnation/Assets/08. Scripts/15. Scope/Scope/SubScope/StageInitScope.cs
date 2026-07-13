using Manager;
using Player.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageInitScope : LifetimeScope
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CameraManager _cameraManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_playerController);
        builder.RegisterComponent(_cameraManager);
    }
}
