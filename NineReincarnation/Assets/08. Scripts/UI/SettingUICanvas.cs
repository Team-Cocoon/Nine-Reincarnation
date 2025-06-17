using UnityEngine;
using UnityEngine.UI;

public class SettingUICanvas : MonoBehaviour
{
    [Header("--- 버튼 ---")]
    [SerializeField] private Button _exitButton; //옵션 닫기 버튼
    [SerializeField] private Button _endButton; //게임종료 버튼

    [Header("--- 사운드 조절 ---")]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;

    [Header("--- UI ---")]
    [SerializeField] private GameObject _settingUI;
    [SerializeField] private GameObject _button;

    private void Awake()
    {
        _settingUI.SetActive(false);
        _button.SetActive(true);

        UIEventHandler.OnOpenSettingUI += OpenSettingUI;
        UIEventHandler.OnCloseSettingUI += CloseSettingUI;
    }

    private void Start()
    {
        _sfxSlider.value = SoundEventHandler.OnReturnSfxVolmue;
        _bgmSlider.value = SoundEventHandler.OnReturnBgmVolmue;

        _button.GetComponent<Button>().onClick.AddListener(Toggle);
        _exitButton.onClick.AddListener(ExitButtonEvent);
        _endButton.onClick.AddListener(EndButtonEvent);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    private void OnDestroy()
    {
        _button.GetComponent<Button>().onClick.RemoveListener(Toggle);
        _exitButton.onClick.RemoveListener(ExitButtonEvent);
        _endButton.onClick.RemoveListener(EndButtonEvent);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);

        UIEventHandler.OnOpenSettingUI -= OpenSettingUI;
        UIEventHandler.OnCloseSettingUI -= CloseSettingUI;
    }

    private void Toggle()
    {
        UIEventHandler.ToggleSettingUI();
    }

    private void OpenSettingUI()
    {
        PlayClickSound();
        _settingUI.SetActive(true);
        _button.SetActive(false);
    }

    private void CloseSettingUI()
    {
        PlayClickSound();
        _settingUI.SetActive(false);
        _button.SetActive(true);
    }

    private void ExitButtonEvent()
    {
        PlayClickSound();
        Toggle();
    }

    private void EndButtonEvent()
    {
        PlayClickSound();
        Application.Quit();
    }

    private void PlayClickSound()
    {
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.Click);
    }

    private void OnSfxSliderChanged(float value)
    {
        SoundEventHandler.OnUpdateSfxVolmue(value);
    }

    private void OnBgmSliderChanged(float value)
    {
        SoundEventHandler.OnUpdateBgmVolmue(value);
    }
}
