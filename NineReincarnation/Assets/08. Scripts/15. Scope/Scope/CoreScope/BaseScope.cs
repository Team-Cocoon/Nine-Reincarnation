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

    [Header("----- UI Starter ------")]
    [SerializeField] private GameObject _loadingScreen;

    protected override void Configure(IContainerBuilder builder)
    {
        // MonoBehaviour 컴포넌트 등록
        builder.RegisterComponent(_sceneDataManager);
        builder.RegisterComponent(_saveManager);
        builder.RegisterComponent(_settingUI);
        builder.RegisterComponent(_fadeUI);
        builder.RegisterComponent(_dialogueUI);
        builder.RegisterComponent(_loadingScreen);

        // 매니저(싱글톤) 등록
        builder.Register<UIManager>(Lifetime.Singleton);
        
        // 씬 전환 관련 클래스 등록
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<SceneTransitionManager>(Lifetime.Singleton);
        builder.Register<TransitionTaskRegistry>(Lifetime.Singleton);

        // 초기화 및 엔트리 포인트 등록
        // VContainer가 알아서 SceneTransitionManager와 SceneDataManager를 주입
        builder.Register<CoreInitiator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        
        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton).AsSelf();
    }
}