using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXSlideHandle : Slider, IEndDragHandler
{
    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (handleRect != null && !RectTransformUtility.RectangleContainsScreenPoint(handleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
        }

        base.OnPointerDown(eventData);
    }
}
