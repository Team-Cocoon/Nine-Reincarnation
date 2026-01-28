using UnityEngine;
using UnityEngine.UI;

public class ClearUI : MonoBehaviour
{
    [Header("---- Button ----")]
    [SerializeField] private Button _titleButton;

    private void Start()
    {
        _titleButton.onClick.AddListener(GameEvent_Title);
    }

    private void OnDestroy()
    {
        _titleButton.onClick.RemoveAllListeners();
    }

    private void GameEvent_Title()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);

        GameEventHandler.TitleExcuted_Invoke();
    }
}

