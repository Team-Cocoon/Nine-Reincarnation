using System.Collections.Generic;
using Player.Action;
using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("---플레이어 정보들---")]
    [SerializeField] private List<PlayerController> _players;

    private string _curActionMap;
    private int _curPlayerIdx = 0;
    private PlayerController _curPlayer;
    private PlayerInput _playerInput;
    private PlayerAction _playerAction;
    private bool _isForce = false;

    public PlayerController CurPlayer => _curPlayer;

    [Inject]
    public void Construct(PlayerController player)
    {
        if (_players.Count > 0)
        {
            _players[0] = player;
        }
    }

    private void Awake()
    {
        Instance = this;

        _playerInput = GetComponent<PlayerInput>();
        _playerAction = GetComponent<PlayerAction>();

        _curPlayer = _players[_curPlayerIdx];
        _playerAction.SetPlayer(_curPlayer);

        _curActionMap = _playerInput.defaultActionMap;

        InputEventHandler.OnChangedActionToUI += InputEvent_OnChangedActionToUI;
        InputEventHandler.OnChangedActionToPlayer += InputEvent_OnChangedActionToPlayer;
        InputEventHandler.OnChangedForceActionToUI += InputEvent_OnChangedForceActionToUI;
        InputEventHandler.OnChangedForceActionToPlayer += InputEvent_OnChangedForceActionToPlayer;
    }

    private void OnDestroy()
    {
        InputEventHandler.OnChangedActionToUI -= InputEvent_OnChangedActionToUI;
        InputEventHandler.OnChangedActionToPlayer -= InputEvent_OnChangedActionToPlayer;
        InputEventHandler.OnChangedForceActionToUI -= InputEvent_OnChangedForceActionToUI;
        InputEventHandler.OnChangedForceActionToPlayer -= InputEvent_OnChangedForceActionToPlayer;
    }

    private void InputEvent_OnChangedActionToUI()
    {
        if (_isForce)
        {
            return;
        }
        ChangeActionToUI();
    }
    private void InputEvent_OnChangedActionToPlayer()
    {
        if (_isForce)
        {
            return;
        }
        ChangeActionToPlayer();
    }
    private void InputEvent_OnChangedForceActionToUI()
    {
        _isForce = !_isForce;
        ChangeActionToUI();
    }
    private void InputEvent_OnChangedForceActionToPlayer()
    {
        _isForce = !_isForce;
        ChangeActionToPlayer();
    }

    public void ChangeActionToUI()
    {
        _playerInput.SwitchCurrentActionMap("UI");
    }
    public void ChangeActionToPlayer()
    {
        _playerInput.SwitchCurrentActionMap("Player");
    }

    /// <summary>
    /// 플레이어 변경
    /// </summary>
    public void Swap(string name)
    {
        //_currentIdx = (_currentIdx + 1) % _playerNames.Count;
        //string playeName = _playerNames[_currentIdx];

        //_action.SetPlayer(playeName, _players[playeName]);

        //CameraManager.Instance.ChangeTarget(_players[playeName].GetTransform());
    }
}
