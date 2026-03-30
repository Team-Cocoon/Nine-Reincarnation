using ExcelData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BaseScope : LifetimeScope
{
    [Header("----- Audio -----")]
    [SerializeField] private AudioManager _audioManager;

    [Header("----- Scene -----")]
    [SerializeField] private SceneDataManager _sceneDataManager;

    [Header("----- Save -----")]
    [SerializeField] private SaveManager _saveManager;

    [Header("----- UI ------")]
    [SerializeField] private FadeUI _fadeUI;
    [SerializeField] private SettingUI _settingUI;
    [SerializeField] private DialogueUI _dialogueUI;

    [Header("----- Data ------")]
    [SerializeField] private DialogueDataSO _dialogueData;


    [Header("----- CoreInitiator ------")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private string _scenePath;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_sceneDataManager);
        builder.RegisterComponent(_saveManager);
        builder.RegisterComponent(_settingUI);
        builder.RegisterComponent(_fadeUI);
        builder.RegisterComponent(_dialogueUI);

        builder.RegisterComponent(_loadingScreen);
        builder.RegisterComponent(_scenePath);

        //UIManager 싱글톤 등록
        builder.Register<UIManager>(Lifetime.Singleton);

        builder.Register<CoreSceneLoader>(Lifetime.Singleton).As<CoreSceneLoader>();

#if UNITY_EDITOR
        if(!CoreBootStrap.RequestedStartSceneName.Contains("BaseScene"))
        {
            _scenePath = CoreBootStrap.RequestedStartScenePath;
        }
#endif
        builder.Register<CoreInitiator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf()
        .WithParameter("loadingScreen", _loadingScreen)
        .WithParameter("scenePath", _scenePath);

        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton).AsSelf();
    }
}
