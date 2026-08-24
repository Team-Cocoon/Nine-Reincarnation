using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TestUI
{
    [Header("--- 버튼 ---")]
    [SerializeField] private Button _exitButton; //옵션 닫기 버튼
    [SerializeField] private Button _nextButton; //씬 이동 버튼

    [Header("--- Scene Data ---")]
    [SerializeField] private SceneDataSO _sceneData;

    [Header("--- Setting Button ---")]
    [SerializeField] private GameObject _uiToggleButton;

    [Header("--- SceneState ---")]
    [SerializeField] private GameObject           _statePanel;
    [SerializeField] private TMP_Dropdown         _stateDropdown;
    [SerializeField] private List<SceneStateType> _stateList;
    [SerializeField] private SceneStateType       _curState = 0;

    [Header("--- StageState ---")]
    [SerializeField] private GameObject   _stageStatePanel;
    [SerializeField] private TMP_Dropdown _stageStateDropdown;

    [Header("--- StoryState ---")]
    [SerializeField] private GameObject   _storyStatePanel;
    [SerializeField] private TMP_Dropdown _storyStateDropdown;

    [Inject] private CoreSceneLoader _coreSceneLoader;
    [Inject] private SaveManager     _saveManager;

    private GameProgressData _gameData => _saveManager.GameData;

    private int _storyIndex = 0;
    private int _storySubIndex = 0;

    private int _stageIndex = 0;
    private int _stageSubIndex = 0;

    private void InitDropDownOption()
    {
        foreach (TMP_Dropdown.OptionData option in _stateDropdown.options)
        {
            SceneStateType stateType;
            if (Enum.TryParse(option.text, out stateType))
            {
                _stateList.Add(stateType);
            }
            else
            {
                continue;
            }
        }

        _stageStateDropdown.ClearOptions();
        for (int i = 0; i < _sceneData.StageScene.Size; ++i)
        {
            for (int j = 1; j < _sceneData.StageScene.SubSceneGroups[i].Size; ++j)
            {
                _stageStateDropdown.options.Add(new TMP_Dropdown.OptionData($"{i + 1}-{j}"));
            }
        }


        _storyStateDropdown.ClearOptions();
    }

    private void SceneStateChange(int x)
    {
        if (_stateList[x] != _curState)
        {
            _curState = _stateList[x];

            switch (_curState)
            {
                case SceneStateType.Title:
                case SceneStateType.Clear:
                    _stageStatePanel.SetActive(false);
                    _storyStatePanel.SetActive(false);
                    break;
                case SceneStateType.Stage:
                    _stageStatePanel.SetActive(true);
                    _storyStatePanel.SetActive(false);
                    break;
            }
        }
    }

    private void StageChange(int x)
    {
        string selectedText = _stageStateDropdown.options[x].text;

        string[] parts = selectedText.Split('-');

        if (parts.Length >= 2)
        {
            _stageIndex = int.Parse(parts[0]) - 1;
            _stageSubIndex = int.Parse(parts[1]);
        }
    }

    private void StoryChange(int x)
    {
        string selectedText = _storyStateDropdown.options[x].text;

        string[] parts = selectedText.Split('-');

        if (parts.Length >= 2)
        {
            _storyIndex = int.Parse(parts[0]) - 1;
            _storySubIndex = int.Parse(parts[1]) - 1;
        }
    }

    private void Awake()
    {
        _uiToggleButton.SetActive(true);

        //UIEventHandler.ToggleSettingUI += UIEvent_ToggleUI;
    }

    private void Start()
    {
        InitDropDownOption();

        _stateDropdown.onValueChanged.AddListener(SceneStateChange);
        _storyStateDropdown.onValueChanged.AddListener(StoryChange);
        _stageStateDropdown.onValueChanged.AddListener(StageChange);

        _uiToggleButton.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);

        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _nextButton.onClick.AddListener(NextButtonEvent);
    }

    protected void OnDestroy()
    {
        _stateDropdown.onValueChanged.RemoveListener(SceneStateChange);
        _storyStateDropdown.onValueChanged.RemoveListener(StoryChange);
        _stageStateDropdown.onValueChanged.RemoveListener(StageChange);

        _uiToggleButton.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);
        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _nextButton.onClick.RemoveListener(NextButtonEvent);

        //UIEventHandler.ToggleSettingUI -= UIEvent_ToggleUI;
    }

    private void ButtonEvent_SettingUI()
    {
        //UIEvent_ToggleUI();
    }

    private void NextButtonEvent()
    {
        //UIEvent_ToggleUI();

        switch (_curState)
        {
            case SceneStateType.Title:
                GameEventHandler.TitleExcuted_Invoke();
                break;
            case SceneStateType.Stage:
                _saveManager.SetSaveData(0);
                _gameData.StageIndex = _stageIndex;
                _gameData.StageSubIndex = _stageSubIndex;
                GameEventHandler.StageExcuted_Invoke();
                break;
            case SceneStateType.Clear:
                GameEventHandler.GameClearExcuted_Invoke();
                break;
        }
    }
}
