using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXSlideHandle : Slider, IEndDragHandler
{
    private bool playedOnDown = false;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playedOnDown)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
        }

        playedOnDown = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (handleRect != null && !RectTransformUtility.RectangleContainsScreenPoint(handleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
            playedOnDown = true;
        }

        base.OnPointerDown(eventData);
    }
}

