using Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ClearScope : LifetimeScope
{
    [SerializeField] private CameraManager _cameraManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<CameraManager>(_cameraManager);
    }
}
