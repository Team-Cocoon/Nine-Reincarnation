using Player.Controller;
using UnityEngine;

public class Feather : MonoBehaviour, IClickInteractableToggle, IHoverInteractableToggle
{
    [Header("----- 아웃라인 조절 ------")]
    [SerializeField] private DrawOutline _outline;
    [ColorUsage(true, true)] //HDR 사용 여부
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activatedColor;

    [Header("----- 상호작용 조절 ------")]
    [SerializeField] private bool _possibleActive;
    [SerializeField] private bool _isActivated;

    [Header("----- 플레이어 ------")]
    [SerializeField] PlayerController _player;

    private LayerMask _playerMask;

    private void Awake()
    {
        _playerMask = LayerMask.GetMask("Player");
    }

    private void ActivateFeather()
    {
        _isActivated = true;
        _player.BecomeLighter();
    }

    private void InactivateFeather()
    {
        _isActivated = false;
        _player.InitGravity();
    }

    public void EnableClickInteraction()
    {
        if (_possibleActive)
        {
            if(_isActivated) //이미 활성화 되어있다면
            {
                _outline.OutlineColor = _activeColor;
                InactivateFeather();
            }
            else
            {
                _outline.OutlineColor = _activatedColor;
                ActivateFeather();
            }
        }
    }
    public void EnableHoverInteraction()
    {
        if (_possibleActive)
        {
            _outline.OutlineColor = _activeColor;
        }
        else
        {
            _outline.OutlineColor = _inactiveColor;
        }
    }

    public void DisableClickInteraction()
    {
        return;
    }

    public void DisableHoverInteraction()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool _detectedPlayer = ((1 << collision.gameObject.layer) & _playerMask) != 0;
        if (_detectedPlayer)
        {
            _outline.OutlineColor = _activeColor;
            _possibleActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool _detectedPlayer = ((1 << collision.gameObject.layer) & _playerMask) != 0;
        if (_detectedPlayer)
        {
            _outline.OutlineColor = _inactiveColor;
            _possibleActive = false;
            InactivateFeather();
        }
    }
}
