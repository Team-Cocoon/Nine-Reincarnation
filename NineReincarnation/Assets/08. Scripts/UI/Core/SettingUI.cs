using System;
using System.Collections.Generic;
using Manager;
using State.SceneState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

public class SettingUI : ToggleUI
{
    [Header("--- 버튼 ---")]
    [SerializeField] private Button _exitButton; //옵션 닫기 버튼
    [SerializeField] private Button _endButton; //게임종료 버튼

    [Header("--- 사운드 조절 ---")]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;

    [Header("--- SettingButton ---")]
    [SerializeField] private GameObject _uiToggleButton;

    [Header("--- 해상도 조절 ---")]
    [SerializeField] private GameObject _resolutionPanel;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private List<Resolution> _resolutionList;
    [SerializeField] private int resolutionNum;
    [SerializeField] private Toggle _screenToggle;
    [SerializeField] private FullScreenMode _screenMode;

    private List<Resolution> _resolutions = new List<Resolution>();
    private int width = 1920;
    private int height = 1080;

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
            width  = _resolutionList[x].width;
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

        UIEventHandler.ToggleSettingUI += UIEvent_ToggleUI;
    }

    protected override void Start()
    {
        base.Start();

        _sfxSlider.value = AudioManager.Instance.VolumData.SfxVolume;
        _bgmSlider.value = AudioManager.Instance.VolumData.BgmVolume;

#if UNITY_WEBGL
        _resolutionPanel.SetActive(false); //웹 빌드 시 해상도 관련 패널 완전닫음
#else
        InitResolution();
        ResolutionChange(0);
        ScreenModeChange(true);
        _screenToggle.onValueChanged      .AddListener(ScreenModeChange);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionChange);
#endif

        _uiToggleButton.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);

        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _endButton.onClick .AddListener(TitleButtonEvent);

        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

#if UNITY_WEBGL


#else
        _screenToggle.onValueChanged.RemoveListener(ScreenModeChange);
        _resolutionDropdown.onValueChanged.RemoveListener(ResolutionChange);
#endif

        _uiToggleButton.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);
        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _endButton.onClick.RemoveListener(TitleButtonEvent);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);

        UIEventHandler.ToggleSettingUI -= UIEvent_ToggleUI;
    }


    private void ButtonEvent_SettingUI()
    {
        PlayClickSound();
        UIEvent_ToggleUI();
    }

    private void TitleButtonEvent()
    {
        PlayClickSound();
        UIEvent_ToggleUI();
        if (SceneStateManager.Instance.CurrentSceneState != SceneState.Title)
        {
            GameEventHandler.TitleExcuted_Invoke();
        }
    }

    private void PlayClickSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
    }

    private void OnSfxSliderChanged(float value)
    {
        SoundEventHandler.OnUpdateSfxVolmue_Invoke(value);
    }

    private void OnBgmSliderChanged(float value)
    {
        SoundEventHandler.OnUpdateBgmVolmue_Invoke(value);
    }
}
