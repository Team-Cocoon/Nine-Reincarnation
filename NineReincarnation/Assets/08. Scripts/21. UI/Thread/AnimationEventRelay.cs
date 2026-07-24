using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [Header("연결할 메인 UI 스크립트")]
    [SerializeField] private BlueThreadSlotUI _targetSlotUI;

    public void AnimEvent_ChangeConnectionIcon()
    {
        if (_targetSlotUI != null)
        {
            _targetSlotUI.ChangeConnectionIcon();
        }
    }
}