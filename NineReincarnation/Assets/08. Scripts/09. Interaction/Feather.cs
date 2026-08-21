using System.Collections.Generic;
using Player.Controller;
using UnityEngine;

public class Feather : DrawOutline, IThreadInteractable
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
    
    [SerializeField] private float _throwDistance = 5f; 
    [SerializeField] private ThreadCompatibility _allowedThreads = ThreadCompatibility.Red;

    [Header("----- Sprite -----")]
    [SerializeField] private SpriteRenderer _sprite;

    private Vector2 _playerPosition => InputManager.Instance.CurPlayer.transform.position;

    public bool IsClickControlToSelf { get => _isClickControlToSelf; }
    public override bool IsHoverControlToSelf { get => _isHoverControlToSelf; }

    PlayerController _player => InputManager.Instance.CurPlayer;
    public ThreadCompatibility AllowedThreads => _allowedThreads;

    private LayerMask _playerMask;
    private Vector2 _lastPosition;
    
    // 추가: 마우스 호버 상태를 추적하기 위한 변수
    private bool _isHovered = false; 

    protected override void Awake()
    {
        base.Awake();
        _lastPosition = transform.position;
        _possibleActive = false;
        _isActivated = false;
        _playerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        SetSpriteDirection();

        // IsTotalActive 제거 (자기 자신이 켜졌을 때만 켜지도록)
        if (_isActivated) 
        {
            IsOutline = true; // 진행 중일 때는 마우스 위치와 상관없이 아웃라인 켜짐
            OutlineColor = _activatedColor;
            return;
        }

        // 거리 계산 및 가능 여부 갱신
        float dist = Vector2.Distance(_playerPosition, transform.position);
        _possibleActive = (dist <= _throwDistance);

        // 상태 1 & 2: 마우스 호버 상태에 따른 아웃라인 표시 처리
        if (!_isHovered)
        {
            // 마우스가 올라가 있지 않으면 무조건 아웃라인 끔
            IsOutline = false;
        }
        else
        {
            // 마우스가 올라가 있을 때만 아웃라인 켬
            IsOutline = true;
            
            // 거리 내부에 있으면 상호작용 가능 색상, 외부에 있으면 불가능 색상
            OutlineColor = _possibleActive ? _activeColor : _inactiveColor;
        }
    }

    private void SetSpriteDirection()
    {
        float moveX = transform.position.x - _lastPosition.x;

        if (Mathf.Abs(moveX) > float.Epsilon)
        {
            _sprite.flipX = moveX > 0;
        }
        _lastPosition = transform.position;
    }

    private void ActivateFeather()
    {
        OutlineColor = _activatedColor;
        IsTotalActive = true;
        _isActivated = true;
        _player.BecomeLighter();
    }

    public void InactivateFeather()
    {
        OutlineColor = _activeColor;
        IsTotalActive = false;
        _isActivated = false;
        _player.InitGravity();
    }

    public void OnThreadHit(ThreadType threadType)
    {
        switch (threadType)
        {
            case ThreadType.Red:
                EnableClickInteraction();
                break;
            case ThreadType.Blue:
                break;
        }
    }

    public void EnableClickInteraction()
    {
        if (_isActivated || IsTotalActive) 
        {
            foreach (Feather feather in ActiveFeather)
            {
                feather.InactivateFeather();
            }
            ActiveFeather.Clear();
        }
        else if (_possibleActive) 
        {
            ActiveFeather.Add(this);
            ActivateFeather();
        }
    }

    public override void EnableHoverInteraction()
    {
        _isHovered = true; // 호버 진입 시 상태 저장

        if (_isActivated) return;
        base.EnableHoverInteraction();
    }

    public void DisableClickInteraction() { return; }

    public override void DisableHoverInteraction()
    {
        _isHovered = false; // 호버 이탈 시 상태 저장

        if (_isActivated) return;
        base.DisableHoverInteraction();
    }
}

