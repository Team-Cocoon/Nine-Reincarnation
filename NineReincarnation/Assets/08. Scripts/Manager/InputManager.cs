using System;
using System.Collections.Generic;
using Manager.Camera;
using Player.Action;
using Player.Controller;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("---처음 움직일 플레이어---")]
    [SerializeField] string _playerName = "Anna";

    private int _currentIdx = 0;
    private List<string> _playerNames = new List<string>();
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private PlayerAction _action;

    public PlayerAction Action => _action;

    private void Awake()
    {
        Instance = this;
        _action = GetComponent<PlayerAction>();
    }

    private void Start()
    {
        Init();
    }

    //데이터 실행
    private void Init()
    {
        PlayerController[] pc = GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        for(int i = 0; i < pc.Length; ++i)
        {
            AddPlayer(pc[i].PlayerName, pc[i]);
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

        CameraManager.Instance.ChangeTarget(_players[playeName].GetTransform());
    }
}
