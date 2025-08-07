using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        GameEventHandler.Start?.Invoke();
    }

    private void GameEvent_Option()
    {
        //UIEventHandler
    }
    private void GameEvent_Exit()
    {
        GameEventHandler.ExitExcuted?.Invoke();
    }
}
