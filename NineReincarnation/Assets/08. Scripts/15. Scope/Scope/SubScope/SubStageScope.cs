using Player.Controller;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SubStageScope : LifetimeScope
{
    [SerializeField] private VirtualCameraManager _vCammanager;
    [SerializeField] private CheckPoint _checkPoint;
    [SerializeField] private NextScene _nextScene;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_vCammanager);
        builder.RegisterComponent(_checkPoint);
        builder.RegisterComponent(_nextScene);

        builder.Register<StageSubInitiator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton).AsSelf();
    }
}
