using Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TitleScope : LifetimeScope
{
    [SerializeField] private TitleUI       _titleUI;
    [SerializeField] private CameraManager _cameraManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<TitleUI>(_titleUI);
        builder.RegisterComponent<CameraManager>(_cameraManager);
    }
}
