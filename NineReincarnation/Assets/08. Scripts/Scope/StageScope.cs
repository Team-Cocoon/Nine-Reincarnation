using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageScope : LifetimeScope
{
    [SerializeField] private VirtualCameraManager _vCammanager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<VirtualCameraManager>(_vCammanager);
    }
}
