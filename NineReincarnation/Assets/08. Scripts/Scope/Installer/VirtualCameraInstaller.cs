using UnityEngine;
using VContainer;
using VContainer.Unity;

public class VirtualCameraInstaller : IInstaller
{
    private VirtualCameraManager _vCammanager;

    public VirtualCameraInstaller(VirtualCameraManager vCammanager)
    {
        _vCammanager = vCammanager;
    }

    public void Install(IContainerBuilder builder)
    {
        builder.RegisterComponent<VirtualCameraManager>(_vCammanager);
    }
}
