using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class StageCoreScope : LifetimeScope
{
    [Header("----- Cheat ------")]
    [SerializeField] private CheatManager _cheatManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<StageManager>(Lifetime.Singleton);

        _cheatManager.gameObject.SetActive(_cheatManager.IsCheatOn);
        if (_cheatManager.IsCheatOn)
        {
            builder.RegisterComponent(_cheatManager);
        }
    }
}
