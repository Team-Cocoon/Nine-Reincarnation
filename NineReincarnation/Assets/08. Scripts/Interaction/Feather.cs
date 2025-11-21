using System.Collections.Generic;
using Player.Controller;
using UnityEngine;

public class Feather : DrawOutline, IClickInteractableToggle
{
    public static bool IsTotalActive;
    public static List<Feather> ActiveFeather = new List<Feather>();

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activatedColor;

    [Header("----- 상호작용 조절 ------")]
    [SerializeField] private bool _possibleActive;
    [SerializeField] private bool _isActivated;
    [SerializeField] private bool _isClickControlToSelf;
    [SerializeField] private bool _isHoverControlToSelf;
    [SerializeField] private float _interactionDistance;
    private Vector2 _playerPosition => InputManager.Instance.CurPlayer.transform.position;

    public bool          IsClickControlToSelf { get => _isClickControlToSelf; }
    public override bool IsHoverControlToSelf { get => _isHoverControlToSelf; }

    PlayerController _player => InputManager.Instance.CurPlayer;

    private LayerMask _playerMask;

    protected override void Awake()
    {
        base.Awake();

        _possibleActive = false;
        _isActivated = false;
        _playerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if(IsOutline == true)
        {
            float dist = Vector2.Distance(_playerPosition, transform.position);

            //상호 작용 불가능한 거리면
            if (dist > _interactionDistance)
            {
                if (!_possibleActive) return;

                OutlineColor    = _inactiveColor;
                _possibleActive = false;

                //이미 상호작용 중이라면
                if(_isActivated)
                {
                    InactivateFeather();
                    IsOutline = false;
                }
            }
            //상호 작용 가능한 거리면
            else
            {
                if (_isActivated || _possibleActive) return;

                 OutlineColor   = _activeColor;
                _possibleActive = true;
            }
        }
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
        OutlineColor = _activeColor;
        IsTotalActive = false;
        _isActivated = false;
        _player.InitGravity();
    }

    public void EnableClickInteraction()
    {
        //상호 작용 가능하면 상호작용 실행
        if (_possibleActive)
        {
            if (_isActivated || IsTotalActive) //이미 활성화 되어있다면
            {
                //활성화 해제
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
        //이미 상호작용 중이면 리턴
        if (_isActivated)
        {
            return;
        }

        //상호 작용 가능하면 색 변경
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
        //이미 상호작용 중이라면 리턴
        if (_isActivated)
        {
            return;
        }

        base.DisableHoverInteraction();
        return;
    }
}
