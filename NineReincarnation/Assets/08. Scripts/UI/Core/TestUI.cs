using System;
using System.Collections.Generic;
using State.SceneState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TestUI : ToggleUI
{
    [Header("--- 버튼 ---")]
    [SerializeField] private Button _exitButton; //옵션 닫기 버튼
    [SerializeField] private Button _nextButton; //씬 이동 버튼

    [Header("--- Scene Data ---")]
    [SerializeField] private SceneDataSO _sceneData;

    [Header("--- Setting Button ---")]
    [SerializeField] private GameObject _uiToggleButton;

    [Header("--- SceneState ---")]
    [SerializeField] private GameObject _statePanel;
    [SerializeField] private TMP_Dropdown _stateDropdown;
    [SerializeField] private List<SceneStateType> _stateList;
    [SerializeField] private int resolutionNum;
    [SerializeField] private Toggle _screenToggle;
    [SerializeField] private FullScreenMode _screenMode;

    [Header("--- StageState ---")]
    [SerializeField] private GameObject _stageStatePanel;
    [SerializeField] private TMP_Dropdown _stageStateDropdown;

    [Header("--- StoryState ---")]
    [SerializeField] private GameObject _storyStatePanel;
    [SerializeField] private TMP_Dropdown _storyStateDropdown;

    [Inject] private CoreSceneLoader _coreSceneLoader;
    private SceneStateType curState = 0;


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
        for(int i = 0; i < _sceneData.StageScene.Size; ++i)
        {
            for (int j = 0; j <= _sceneData.StageScene.SubSceneGroups[i].Size; ++j)
            {

            }
        }

        _storyStateDropdown.ClearOptions();
        for (int i = 0; i < _sceneData.StoryScene.Size; ++i)
        {
            for (int j = 0; j <= _sceneData.StoryScene.SubSceneGroups[i].Size; ++j)
            {

            }
        }
    }

    public void ResolutionChange(int x)
    {
        if (_stateList[x] != curState)
        {
            curState = _stateList[x];
        }
    }

    private void Awake()
    {
        _uiToggleButton.SetActive(true);

        UIEventHandler.ToggleSettingUI += UIEvent_ToggleUI;
    }

    private void Start()
    {
        _uiToggleButton.GetComponent<Button>().onClick.AddListener(ButtonEvent_SettingUI);

        _exitButton.onClick.AddListener(ButtonEvent_SettingUI);
        _nextButton.onClick.RemoveListener(NextButtonEvent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _stateDropdown.onValueChanged.RemoveListener(ResolutionChange);

        _uiToggleButton.GetComponent<Button>().onClick.RemoveListener(ButtonEvent_SettingUI);
        _exitButton.onClick.RemoveListener(ButtonEvent_SettingUI);
        _nextButton.onClick.RemoveListener(NextButtonEvent);

        UIEventHandler.ToggleSettingUI -= UIEvent_ToggleUI;
    }

    private void ButtonEvent_SettingUI()
    {
        UIEvent_ToggleUI();
    }

    private void NextButtonEvent()
    {;
        UIEvent_ToggleUI();

        switch (curState)
        {
            case SceneStateType.Title:
                GameEventHandler.TitleExcuted_Invoke();
                break;
            case SceneStateType.Story:

                break;
            case SceneStateType.Stage:

                break;
            case SceneStateType.Clear:
                GameEventHandler.GameClearExcuted_Invoke();
                break;
        }
    }
}
