using System;
using System.Collections.Generic;
using Player.Action;
using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInfo
{
    public string _name;
    public PlayerController _player;
}


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("---처음 움직일 플레이어---")]
    [SerializeField] string _playerName = "Anna";

    [Header("---플레이어 정보들---")]
    [SerializeField] private List<PlayerInfo> _playerInfo;

    private int _currentIdx = 0;
    private List<string> _playerNames = new List<string>();
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private PlayerAction _action;
    public PlayerAction Action => _action;
    public Dictionary<string, PlayerController> Players => _players;

    private void Awake()
    {
        Instance = this;
        _action = GetComponent<PlayerAction>();

        SceneEventHandler.SceneStarted += Init;
    }

    private void OnDestroy()
    {
        SceneEventHandler.SceneStarted -= Init;
    }

    public void DisableInput()
    {
        _action.DisableInput();
    }
    public void EnableInput()
    {
        _action.EnableInput();
    }


    //데이터 실행
    private void Init()
    {
        for (int i = 0; i < _playerInfo.Count; ++i)
        {
            AddPlayer(_playerInfo[i]._name, _playerInfo[i]._player);
        }

        _currentIdx = _playerNames.IndexOf(_playerName);
        _action.SetPlayer(_playerName, _players[_playerName]);
    }

    private void Clear()
    {
        _playerNames.Clear();
        _players.Clear();
    }

    /// <summary>
    /// 플레이어 추가
    /// </summary>
    /// <param name="name"></param>
    /// <param name="controller"></param>
    public void AddPlayer(string name, PlayerController controller)
    {
        _playerNames.Add(name);
        _players.Add(name, controller);
    }

    /// <summary>
    /// 플레이어 변경
    /// </summary>
    public void Swap(string name)
    {
        _currentIdx = (_currentIdx + 1) % _playerNames.Count;
        string playeName = _playerNames[_currentIdx];

        _action.SetPlayer(playeName, _players[playeName]);

        //CameraManager.Instance.ChangeTarget(_players[playeName].GetTransform());
    }
}
