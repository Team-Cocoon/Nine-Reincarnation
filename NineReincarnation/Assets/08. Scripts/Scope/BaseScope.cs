using StateMachine.SceneStateMachine;
using UnityEngine;
using Utilities;
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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<CoreSceneLoader>(_coreSceneLoader);
        builder.RegisterComponent<SceneDataManager>(_sceneDataManager);
        builder.RegisterComponent<SaveManager>(_saveManager);
        builder.RegisterComponent<FadeUI>(_fadeUI);
        builder.RegisterComponent<UIManager>(_uiManager);
        builder.RegisterComponent<Camera>(_uiCamera);
        builder.RegisterComponent<SettingUI>(_settingUI);
        builder.RegisterComponent<AudioManager>(_audioManager);
    }
}
