using UnityEngine;
using Utilities;
using VContainer;
using VContainer.Unity;

public class BaseScope : LifetimeScope
{
    [SerializeField] private CoreSceneLoader _coreSceneLoader;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<CoreSceneLoader>(_coreSceneLoader);
    }
}
