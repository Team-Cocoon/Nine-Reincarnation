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

    protected override void Configure(IContainerBuilder builder)
    {
        //UIManager 싱글톤 등록
        builder.Register<UIManager>(Lifetime.Singleton);

        builder.RegisterEntryPoint<CoreSceneLoader>(Lifetime.Singleton).As<CoreSceneLoader>();

        builder.Register<CoreInitiator>(Lifetime.Scoped).AsSelf();   
        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Scoped).AsSelf();
    }
}
