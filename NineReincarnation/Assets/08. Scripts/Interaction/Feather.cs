using System.Collections.Generic;
using Player.Controller;
using UnityEngine;

public class Feather : DrawOutline, IClickInteractableToggle, IHoverInteractableToggle
{
    public static bool IsTotalActive;
    public static List<Feather> ActiveFeather = new List<Feather>();

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
        OutlineColor = _activatedColor;
        IsTotalActive = true;
        _isActivated = true;
        _player.BecomeLighter();
    }

    private void InactivateFeather()
    {
        IsOutline = false;
        OutlineColor = _activeColor;
        IsTotalActive = false;
        _isActivated = false;
        _player.InitGravity();
    }

    public void EnableClickInteraction()
    {
        if (_possibleActive)
        {
            if (_isActivated || IsTotalActive) //이미 활성화 되어있다면
            {//활성화 해제
                foreach (Feather feather in ActiveFeather)
                {
                    feather.InactivateFeather();
                }
                ActiveFeather.Clear();

                InactivateFeather();
            }
            else
            {//활성화

                ActiveFeather.Add(this);
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
