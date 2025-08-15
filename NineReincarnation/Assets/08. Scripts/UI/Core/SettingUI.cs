using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

        _button.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);
        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _endButton.onClick.AddListener(EndButtonEvent);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _button.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);
        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _endButton.onClick.RemoveListener(EndButtonEvent);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);

        UIEventHandler.ToggleSettingUI -= UIEvent_ToggleUI;
    }


    private void ButtonEvent_SettingUI()
    {
        PlayClickSound();
        UIEvent_ToggleUI();
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
