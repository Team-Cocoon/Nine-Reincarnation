using UnityEngine;
using VContainer;
public class ToggleUI : MonoBehaviour
{
    [Header("------ UI ------")]
    [SerializeField] private string _name;
    [SerializeField] private GameObject _ui;
    private UIManager _uiManager;

    [Inject]
    private void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
        _uiManager.AddUIDictionary(_name, _ui);
    }
    public string Name => _name;
    public GameObject UI => _ui;
    private void Awake()
    {
        _ui.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        _uiManager.RemoveUIDictionary(_name);
    }

    protected virtual void UIEvent_ToggleUI()
    {
        _uiManager.ToggleUI(name, true);
    }
}