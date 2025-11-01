using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageScope : LifetimeScope
{
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<VCamArea>(Lifetime.Scoped);
    }
}
