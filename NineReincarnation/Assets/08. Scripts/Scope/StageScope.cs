using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageScope : LifetimeScope
{
    [SerializeField] private VirtualCameraManager _vCammanager;
    [SerializeField] private StoryEventManager _storyEventmanager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<VirtualCameraManager>(_vCammanager);
        //builder.RegisterComponent<StoryEventManager>(_storyEventmanager);
    }
}
