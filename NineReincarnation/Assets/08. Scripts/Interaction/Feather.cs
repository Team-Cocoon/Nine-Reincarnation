using Player.Controller;
using UnityEngine;

public class Feather : DrawOutline, IClickInteractableToggle, IHoverInteractableToggle
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activatedColor;

    [Header("----- 상호작용 조절 ------")]
    [SerializeField] private bool _possibleActive;
    [SerializeField] private bool _isActivated;

    PlayerController _player => InputManager.Instance.Action.Player;

    private LayerMask _playerMask;

    protected override void Awake()
    {
        base.Awake();

        _possibleActive = false;
        _isActivated = false;
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
            if (_isActivated) //이미 활성화 되어있다면
            {
                OutlineColor = _activeColor;
                InactivateFeather();
            }
            else
            {
                OutlineColor = _activatedColor;
                ActivateFeather();
            }
        }
    }
    public override void EnableHoverInteraction()
    {
        if (_isActivated)
        {
            return;
        }

        if (_possibleActive)
        {
            OutlineColor = _activeColor;
        }
        else
        {
            OutlineColor = _inactiveColor;
        }

        base.EnableHoverInteraction();
    }

    public void DisableClickInteraction()
    {
        return;
    }

    public override void DisableHoverInteraction()
    {
        if (_isActivated)
        {
            return;
        }
        base.DisableHoverInteraction();
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool _detectedPlayer = ((1 << collision.gameObject.layer) & _playerMask) != 0;
        if (_detectedPlayer)
        {
            OutlineColor = _activeColor;
            _possibleActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool _detectedPlayer = ((1 << collision.gameObject.layer) & _playerMask) != 0;
        if (_detectedPlayer)
        {
            OutlineColor = _inactiveColor;
            _possibleActive = false;
            InactivateFeather();
        }
    }
}
