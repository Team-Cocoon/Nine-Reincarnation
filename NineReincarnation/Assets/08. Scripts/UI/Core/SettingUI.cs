using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject _button;

    [Header("--- 해상도 조절 ---")]
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
        _ui.SetActive(false);
        _button.SetActive(true);

        UIEventHandler.ToggleSettingUI += UIEvent_ToggleUI;
    }

    protected override void Start()
    {
        base.Start();

        _sfxSlider.value = AudioManager.Instance.VolumData.SfxVolume;
        _bgmSlider.value = AudioManager.Instance.VolumData.BgmVolume;

        InitResolution();

        ResolutionChange(0);
        ScreenModeChange(true);

        _screenToggle.onValueChanged.AddListener(ScreenModeChange);
        _button.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);
        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionChange);
        _endButton.onClick.AddListener(EndButtonEvent);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _screenToggle.onValueChanged.RemoveListener(ScreenModeChange);
        _button.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);
        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _resolutionDropdown.onValueChanged.RemoveListener(ResolutionChange);
        _endButton.onClick.RemoveListener(EndButtonEvent);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);

        UIEventHandler.ToggleSettingUI -= UIEvent_ToggleUI;
    }


    private void ButtonEvent_SettingUI()
    {
        PlayClickSound();
        UIEvent_ToggleUI(true);
    }

    private void EndButtonEvent()
    {
        PlayClickSound();
        Application.Quit();
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
