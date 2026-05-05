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
        // 이 함수 내부에서 서브맵 인덱스도 0으로 초기화된다고 가정
        _saveManager.SetSaveData(0);

        // SceneDataManager가 판단해서 필요한 3개(또는 2개)의 씬 리스트를 묶어주도록 요청
        List<string> scenesToLoad = _sceneDataManager.GetTargetScenes(0, 0);

        // 씬 전환 매니저에게 전환 요청
        _transitionManager.TransitionToScenes(scenesToLoad).Forget();
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