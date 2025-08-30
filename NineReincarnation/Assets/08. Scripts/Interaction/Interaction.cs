using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("상호작용 여부")]
    [SerializeField] private bool _isInteraction = false;

    private IHoverInteractableToggle[] _hoverInteractableToggles;
    private IClickInteractableToggle[] _clickInteractableToggles;

    public bool IsInteraction
    {
        get => _isInteraction;
        set => _isInteraction = value;
    }

    private void Awake()
    {
        _hoverInteractableToggles = GetComponents<IHoverInteractableToggle>();
        _clickInteractableToggles = GetComponents<IClickInteractableToggle>();
    }

    private void OnHoverInteraction()
    {
        foreach (IHoverInteractableToggle interactableToggle in _hoverInteractableToggles)
        {
            interactableToggle.EnableHoverInteraction();
        }
    }

    private void OffHoverInteraction()
    {
        foreach (IHoverInteractableToggle interactableToggle in _hoverInteractableToggles)
        {
            interactableToggle.DisableHoverInteraction();
        }
    }

    private void OnClickInteraction()
    {
        foreach (IClickInteractableToggle interactableToggle in _clickInteractableToggles)
        {
            interactableToggle.EnableClickInteraction();
        }
    }

    private void OffClickInteraction()
    {
        foreach (IClickInteractableToggle interactableToggle in _clickInteractableToggles)
        {
            interactableToggle.DisableClickInteraction();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            OnClickInteraction();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            OnHoverInteraction();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            OffHoverInteraction();
        }
    }
}
