using UnityEngine;
using UnityEngine.UI;

public class BlueThreadSlotUI : MonoBehaviour
{
    [Header("--- UI References ---")]
    [SerializeField] private Image _threadIcon;
    [SerializeField] private Image _connectionIcon;

    [Header("--- Animation ---")]
    [SerializeField] private Animator _animator;

    private readonly int _useTriggerHash = Animator.StringToHash("Use");

    private bool _isActiveState = true;
    private bool _isConnectedState = true;

    // 애니메이션 이벤트 시점에 적용할 비활성화 이미지 저장용
    private Sprite _cachedDisconnectedSprite;

    // 1. 실(Thread) 이미지 상태 제어
    public void SetThreadState(bool isActive, Sprite activeSprite, Sprite inactiveSprite)
    {
        if (_isActiveState && !isActive)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(_useTriggerHash); // 애니메이션 발동
            }
        }
        else if (!_isActiveState && isActive)
        {
            _threadIcon.sprite = activeSprite;

            // ★ 추가된 부분: 죽고 부활하거나 초기화될 때 애니메이터를 강제로 Entry(기본 상태)로 되돌림
            if (_animator != null)
            {
                _animator.Rebind();
                _animator.Update(0f);
            }
        }

        _isActiveState = isActive;
    }

    // 2. 연결고리 이미지 상태 제어 (이미지만 캐싱해두고 대기)
    public void SetConnectionState(bool isConnected, Sprite connectedSprite, Sprite disconnectedSprite)
    {
        if (!_connectionIcon.gameObject.activeSelf) return;

        if (_isConnectedState && !isConnected)
        {
            // 애니메이션 이벤트가 호출될 때 갈아끼울 수 있도록 이미지를 저장만 해둠
            _cachedDisconnectedSprite = disconnectedSprite;
        }
        else if (!_isConnectedState && isConnected)
        {
            // 활성화될 때는 애니메이션 없이 즉시 복구
            _connectionIcon.sprite = connectedSprite;
        }

        _isConnectedState = isConnected;
    }

    // 3. ★ 애니메이션 이벤트로 실행될 함수 ★
    public void ChangeConnectionIcon()
    {
        // 현재 해제 상태이고, 캐싱해둔 이미지가 있다면 교체
        if (!_isConnectedState && _cachedDisconnectedSprite != null)
        {
            _connectionIcon.sprite = _cachedDisconnectedSprite;
        }
    }

    public void SetConnectionVisibility(bool isVisible)
    {
        _connectionIcon.gameObject.SetActive(isVisible);
    }
}