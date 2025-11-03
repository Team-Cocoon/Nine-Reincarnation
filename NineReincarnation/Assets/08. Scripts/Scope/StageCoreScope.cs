using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    [SerializeField] private Transform _player;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<Transform>(_player);
    }
}
