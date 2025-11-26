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
    [SerializeField] private CoreSceneLoader _coreSceneLoader;

    [Header("----- Save -----")]
    [SerializeField] private SaveManager _saveManager;

    [Header("----- UI ------")]
    [SerializeField] private FadeUI _fadeUI;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private SettingUI _settingUI;

    [Header("----- Data ------")]
    [SerializeField] private DialogueDataSO _dialogueData;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance<DialogueDataSO>(_dialogueData);
        builder.Register<DialogueDB>(Lifetime.Singleton);

        builder.RegisterComponent<CoreSceneLoader>(_coreSceneLoader);
        builder.RegisterComponent<SceneDataManager>(_sceneDataManager);
        builder.RegisterComponent<SaveManager>(_saveManager);
        builder.RegisterComponent<FadeUI>(_fadeUI);
        builder.RegisterComponent<UIManager>(_uiManager);
        builder.RegisterComponent<Camera>(_uiCamera);
        builder.RegisterComponent<SettingUI>(_settingUI);
        builder.RegisterComponent<AudioManager>(_audioManager);

        builder.RegisterBuildCallback(container =>
        {
            // VContainer 빌드가 끝나면, 이 코드를 즉시 실행해라.
            container.Resolve<DialogueDB>();
        });
    }
}
