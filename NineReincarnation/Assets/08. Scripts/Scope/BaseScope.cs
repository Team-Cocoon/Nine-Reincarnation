using UnityEngine;
using Utilities;
using VContainer;
using VContainer.Unity;

public class BaseScope : LifetimeScope
{
    [SerializeField] private SceneDataManager _sceneDataManager;
    [SerializeField] private CoreSceneLoader _coreSceneLoader;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private FadeUI _fadeUI;
    [SerializeField] private UIManager _uiManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<CoreSceneLoader>(_coreSceneLoader);
        builder.RegisterComponent<SceneDataManager>(_sceneDataManager);
        builder.RegisterComponent<SaveManager>(_saveManager);
        builder.RegisterComponent(_fadeUI).As<FadeUI>().As<ToggleUI>();
        builder.RegisterComponent<UIManager>(_uiManager);
    }
}
