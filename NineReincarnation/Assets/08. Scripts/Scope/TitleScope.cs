using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;

public class TitleScope : LifetimeScope
{
    [SerializeField] private Camera _camera;
    [SerializeField] private TitleUI _titleUI;
    protected override void Configure(IContainerBuilder builder)
    {
        Camera cam = Parent.Container.Resolve<Camera>();
        _camera.GetUniversalAdditionalCameraData().cameraStack.Add(cam);

        builder.RegisterComponent<TitleUI>(_titleUI);
    }
}
