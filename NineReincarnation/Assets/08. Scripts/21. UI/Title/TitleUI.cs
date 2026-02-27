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

        _saveManager.SetSaveData(0);

        // if (_saveManager.GameData.State == GameState.Stoty)
        // {
        //     GameEventHandler.StoryExcuted_Invoke();
        // }
        // else if (_saveManager.GameData.State == GameState.Stage)
        // {
        //     GameEventHandler.StageExcuted_Invoke();
        // }
    }

    private void GameEvent_Option()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        //UIEventHandler.ToggleSettingUI_Invoke(true);
    }
    private void GameEvent_Exit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        Application.Quit();
    }
}
