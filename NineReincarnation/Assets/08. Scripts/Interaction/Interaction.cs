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

    private void OnEnable()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 마우스 좌표를 월드 좌표로 변환
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        // 마우스 위치에서 2D Raycast 실행
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            // 마우스가 이미 이 오브젝트 위에 있으면 OnPointerEnter 강제 실행
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerEnterHandler);
        }
    }

    private void OnHoverInteraction()
    {
        foreach (IHoverInteractableToggle interactableToggle in _hoverInteractableToggles)
        {
            if (!interactableToggle.IsHoverControlToSelf)
            {
                interactableToggle.EnableHoverInteraction();
            }
        }
    }

    private void OffHoverInteraction()
    {
        foreach (IHoverInteractableToggle interactableToggle in _hoverInteractableToggles)
        {
            if (!interactableToggle.IsHoverControlToSelf)
            {
                interactableToggle.DisableHoverInteraction();
            }
        }
    }

    private void OnClickInteraction()
    {
        foreach (IClickInteractableToggle interactableToggle in _clickInteractableToggles)
        {
            if (!interactableToggle.IsClickControlToSelf)
            {
                interactableToggle.EnableClickInteraction();
            }
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
        Debug.Log("들어옴");
        if (_isInteraction)
        {
            Debug.Log("실행");
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
