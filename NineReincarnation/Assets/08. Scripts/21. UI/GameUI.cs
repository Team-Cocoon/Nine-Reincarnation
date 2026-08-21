using UnityEngine;
using VContainer;
public class GameUI : MonoBehaviour
{
    [Header("------ UI ------")]
    [SerializeField] private string _name;
    [SerializeField] protected GameObject _ui;
    protected UIManager _uiManager;

    [Inject]
    private void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
        _uiManager.AddUIDictionary(_name, _ui);
        _ui.SetActive(false);
    }

    public string Name => _name;
    public GameObject UI => _ui;

    protected virtual void OnDestroy()
    {
        _uiManager.RemoveUIDictionary(_name);
    }

    public virtual void ToggleUI()
    {
        _ui.SetActive(!_ui.activeSelf);
    }

    public virtual void OpenUI()
    {
        _ui.SetActive(true);
    }

    public virtual void CloseUI()
    {
        _ui.SetActive(false);
    }
}