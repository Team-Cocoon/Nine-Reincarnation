using System;
using System.Collections.Generic;
using System.Linq; // Contains를 사용하기 위해 추가
using Cysharp.Threading.Tasks; // Forget()을 사용하기 위해 추가
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[Serializable]
struct Resolution
{
    public int width;
    public int height;

    public Resolution(int _width, int _height)
    {
        width = _width;
        height = _height;
    }
}

public class SettingUI : GameUI
{
    [Header("--- Button ---")]
    [SerializeField] private Button _exitButton; //옵션 닫기 버튼
    [SerializeField] private Button _titleButton; //타이틀로 돌아가기 버튼 (주석 수정)

    [Header("--- Sound ---")]
    [SerializeField] private SoundVolumeSO _volume;
    [SerializeField] private Slider _totalSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;

    [Header("--- SettingButton ---")]
    [SerializeField] private GameObject _uiToggleButton;

    [Header("--- Resolution ---")]
    [SerializeField] private GameObject _resolutionPanel;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private List<Resolution> _resolutionList;
    [SerializeField] private int resolutionNum;
    [SerializeField] private Toggle _screenToggle;
    [SerializeField] private FullScreenMode _screenMode;

    // [수정된 의존성 주입] 기존 CoreSceneLoader를 제거하고 새 매니저들을 주입받습니다.
    [Inject] private SceneLoader _sceneLoader;
    [Inject] private SceneDataManager _sceneDataManager;
    [Inject] private SceneTransitionManager _transitionManager;

    private List<Resolution> _resolutions = new List<Resolution>();
    private int width = 1920;
    private int height = 1080;

    public override void ToggleUI()
    {
        base.ToggleUI();

        //열때 시간 정지
        Time.timeScale = _ui.activeSelf ? 0 : 1;
    }

    private void InitResolution()
    {
        foreach (TMP_Dropdown.OptionData option in _resolutionDropdown.options)
        {
            string[] parts = option.text.Split(" x ");

            Resolution resolution = new Resolution(int.Parse(parts[0]), int.Parse(parts[1]));
            _resolutionList.Add(resolution);
        }
    }

    public void ResolutionChange(int x)
    {
        if (_resolutionList[x].width != width || _resolutionList[x].height != height)
        {
            width = _resolutionList[x].width;
            height = _resolutionList[x].height;

            Screen.SetResolution(width, height, _screenMode);
        }
    }

    public void ScreenModeChange(bool isFull)
    {
        _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(width, height, _screenMode);
    }

    private void Awake()
    {
        _uiToggleButton.SetActive(true);
    }

    private void Start()
    {
        _totalSlider.value = _volume.MasterVolume;
        _sfxSlider.value = _volume.SfxVolume;
        _bgmSlider.value = _volume.BgmVolume;

#if UNITY_WEBGL && !UNITY_EDITOR
        _resolutionPanel.SetActive(false); //웹 빌드 시 해상도 관련 패널 완전닫음
#else
        InitResolution();
        ResolutionChange(0);
        ScreenModeChange(true);
        _screenToggle.onValueChanged.AddListener(ScreenModeChange);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionChange);
#endif

        _uiToggleButton.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);

        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _titleButton.onClick.AddListener(TitleButtonEvent);

        _totalSlider.onValueChanged.AddListener(OnTotalSliderChanged);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

#if UNITY_WEBGL && !UNITY_EDITOR


#else
        _screenToggle.onValueChanged.RemoveListener(ScreenModeChange);
        _resolutionDropdown.onValueChanged.RemoveListener(ResolutionChange);
#endif

        _uiToggleButton.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);

        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _titleButton.onClick.RemoveListener(TitleButtonEvent);

        _totalSlider.onValueChanged.RemoveListener(OnTotalSliderChanged);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);
    }

    private void ButtonEvent_SettingUI()
    {
        PlayClickSound();
        ToggleUI();
    }

    // [핵심 수정 로직] 타이틀 버튼 클릭 시 동작
    private void TitleButtonEvent()
    {
        PlayClickSound();
        ToggleUI();

        // 1. 현재 로드된 씬 목록 중에 타이틀 씬이 "없는" 경우에만 이동 진행
        if (!_sceneLoader.LoadedScenes.Contains(_sceneDataManager.TitleScene))
        {
            // 2. 타이틀 씬만 로드하도록 매니저에게 요청
            List<string> scenesToLoad = new List<string> { _sceneDataManager.TitleScene };
            _transitionManager.TransitionToScenes(scenesToLoad).Forget();

            // 3. 기존 이벤트 호출 (이벤트 구독자들이 필요한 처리를 할 수 있도록 유지)
            GameEventHandler.TitleExcuted_Invoke();
        }
    }

    private void PlayClickSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
    }

    private void OnTotalSliderChanged(float value)
    {
        _volume.UpdateMasterVolume(value);
    }

    private void OnSfxSliderChanged(float value)
    {
        _volume.UpdateSfxVolume(value);
    }

    private void OnBgmSliderChanged(float value)
    {
        _volume.UpdateBgmVolume(value);
    }
}