using Player.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private ThrowThread _thread;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<PlayerController>(_player);
        builder.RegisterComponent<ThrowThread>(_thread);
    }
}
