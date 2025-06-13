using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private IInteractableToggle[] _interactableToggles;

    private void Awake()
    {
        _interactableToggles = GetComponents<IInteractableToggle>();
    }

    private void OnInteraction()
    {
        foreach(IInteractableToggle interactableToggle in _interactableToggles)
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

    public void OnPointerDown(PointerEventData eventData)
    {
        //여기는 클릭 했을 때 일어날 일 하는거임
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnInteraction();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OffInteraction();
    }
}
