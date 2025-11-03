using System.Threading;
using Player.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TestScope : LifetimeScope
{
    //테스트에 필수적인 것들
    [Header("---- Player ----")]
    [SerializeField] private Transform _start;

    [Header("---- Test Reference ----")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PlayerController _player;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private ThrowThread _thread;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInNewPrefab<AudioManager>(_audioManager, Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<PlayerController>(_player, Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<InputManager>(_inputManager, Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<Camera>(_mainCamera, Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<ThrowThread>(_thread, Lifetime.Singleton);

        builder.RegisterBuildCallback(container =>
        {
            // VContainer 빌드가 끝나면, 이 코드를 즉시 실행해라.
            container.Resolve<Camera>();
            container.Resolve<PlayerController>();
            PlayerController playerInstance = container.Resolve<PlayerController>();
            playerInstance.CheckPoint = _start.position;
            container.Resolve<AudioManager>();
            container.Resolve<InputManager>();
        });
    }
}
