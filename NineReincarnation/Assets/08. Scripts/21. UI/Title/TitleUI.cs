using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TitleUI : MonoBehaviour
{
    [Header("---- Button ----")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

    [Inject] private SaveManager _saveManager;
    [Inject] private SettingUI _settingUI;
    
    // [수정] CoreSceneLoader 대신 새로운 씬 관리 매니저들을 주입받습니다.
    [Inject] private SceneTransitionManager _transitionManager;
    [Inject] private SceneDataManager _sceneDataManager;

    private void Start()
    {
        _startButton.onClick.AddListener(GameEvent_Start);
        _optionButton.onClick.AddListener(GameEvent_Option);
        _exitButton.onClick.AddListener(GameEvent_Exit);
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(GameEvent_Start);
        _optionButton.onClick.RemoveListener(GameEvent_Option);
        _exitButton.onClick.RemoveListener(GameEvent_Exit);
    }

    private void GameEvent_Start()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);

        // 세이브 데이터를 0(첫 스테이지)으로 초기화
        _saveManager.SetSaveData(0);

        // 1. 다음에 열어야 할 씬 리스트 구성 (스테이지 코어 + 1번 스테이지 서브씬들)
        List<string> scenesToLoad = new List<string> { _sceneDataManager.StageCoreScene };
        scenesToLoad.AddRange(_sceneDataManager.GetStageSubScenes(0));

        // 2. 씬 전환 매니저에게 전환 요청 (Diff 로직에 의해 타이틀은 알아서 꺼짐)
        _transitionManager.TransitionToScenes(scenesToLoad).Forget();

        // 3. 기존 이벤트 호출 (다른 UI 닫기, BGM 변경 등 이벤트를 듣고 있는 다른 객체들을 위해 유지)
        GameEventHandler.StageExcuted_Invoke();
    }

    private void GameEvent_Option()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        _settingUI.ToggleUI();
    }

    private void GameEvent_Exit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}