using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TitleUI : MonoBehaviour
{
    [Header("---- 버튼 ----")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

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

        SaveManager.Instance.SetSaveData(0);

        if (SaveManager.Instance.SaveData.State == GameState.Stoty)
        {
            GameEventHandler.StoryExcuted_Invoke();
        }
        else if (SaveManager.Instance.SaveData.State == GameState.Stage)
        {
            GameEventHandler.StageExcuted_Invoke();
        }
    }

    private void GameEvent_Option()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        UIEventHandler.ToggleSettingUI_Invoke();
    }
    private void GameEvent_Exit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        Application.Quit();
    }
}
