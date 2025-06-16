using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("상호작용 여부")]
    [SerializeField] private bool _isInteraction = false;

    private IInteractableToggle[] _interactableToggles;
    private Action _eventAction;

    public bool IsInteraction
    {
        get => _isInteraction;
        set => _isInteraction = value;
    }

    private void Awake()
    {
        _interactableToggles = GetComponents<IInteractableToggle>();
    }

    private void OnInteraction()
    {
        foreach (IInteractableToggle interactableToggle in _interactableToggles)
        {
            interactableToggle.EnableInteraction();
        }
    }

    private void OffInteraction()
    {
        foreach (IInteractableToggle interactableToggle in _interactableToggles)
        {
            interactableToggle.DisableInteraction();
        }
    }
    /// <summary>
    /// 이벤트 함수 등록
    /// </summary>
    /// <param name="action"></param>
    public void SetAction(Action action)
    {
        _eventAction = action;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            _eventAction?.Invoke();
            _eventAction = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            OnInteraction();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isInteraction)
        {
            OffInteraction();
        }
    }
}
