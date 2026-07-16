using Cysharp.Threading.Tasks;
using ExcelData;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct SelectDataStruct
{
    private int _id;
    private int _nextId;
    private string _script;

    public int Id => _id;
    public int NextId => _nextId;
    public string Script => _script;

    public void SetSelectDataStruct(int id, int nextId, string script)
    {
        _id = id;
        _nextId = nextId;
        _script = script;
    }
}

public class SelectButtonInfo
{
    public int ID { get; private set; }
    public Button Button { get; private set; }

    public SelectButtonInfo(int id, Button button)
    {
        ID = id;
        Button = button;
    }

    public void SetInfo(int id, Button button)
    {
        ID = id;
        Button = button;
    }

    public void SetID(int id)
    {
        ID = id;
    }
}


public class SelectUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [SerializeField] private List<SelectButtonInfo> _choiceButtons = new List<SelectButtonInfo>();
    [SerializeField] private SelectUIButtonListener _buttonListner;

    private UniTaskCompletionSource<int> _utcs;

    private SelectClass _currentSelectClass;

    private void Awake()
    {
        _buttonListner?.ConnectSelectUI(this);
    }

    public void ConnectSelecctButtonListener(SelectUIButtonListener buttonListner)
    {
        _buttonListner = buttonListner;
        _buttonListner?.ConnectSelectUI(this);
    }

    public void UpdateUI(SelectClass selectData, SelectDataStruct[] selectDatas)
    {
        if (!string.IsNullOrEmpty(selectData.Script))
        {
            _questionText.gameObject.SetActive(true);
            ChangeScript(_questionText, selectData.Script);
        }
        else
        {
            _questionText.gameObject.SetActive(false);
        }

        _utcs = new UniTaskCompletionSource<int>();

        UpdateQuestion(selectData.Script);

        UpdateChoiceButtons(selectData, selectDatas);

        _currentSelectClass = selectData;

        OpenUI();
    }

    private void UpdateQuestion(string question)
    {
        bool hasQuestion = !string.IsNullOrEmpty(question);

        _questionText.gameObject.SetActive(hasQuestion);

        if (hasQuestion)
        {
            ChangeScript(_questionText, question);
        }
    }

    private void UpdateChoiceButtons(
        SelectClass selectData,
        SelectDataStruct[] selectDatas)
    {
        while (_choiceButtons.Count < selectData.ChoiceCount)
        {
            Button button = Instantiate(
                _choiceButtonPrefab,
                _panel.transform
            ).GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError(
                    "[SelectUI] ChoiceButtonPrefab에 Button 컴포넌트가 없습니다."
                );
                return;
            }

            _choiceButtons.Add(
                new SelectButtonInfo(-1, button)
            );
        }

        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            SelectButtonInfo info = _choiceButtons[i];
            Button button = info.Button;

            if (button == null)
            {
                continue;
            }

            if (i >= selectData.ChoiceCount)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);

            int buttonId = selectDatas[i].Id;
            int nextId = selectDatas[i].NextId;
            string script = selectDatas[i].Script;

            info.SetID(nextId);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log(
                    $"[SelectUI] Button ID: {buttonId}, Next ID: {nextId}"
                );

                _utcs?.TrySetResult(nextId);
            });

            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                ChangeScript(buttonText, script);
            }
        }
    }

    public async UniTask<int> WaitSelect()
    {
        int id = await _utcs.Task;

        CloseUI();

        if(_buttonListner != null)
        {
            id = _buttonListner.OnButtonClicked(id);
        }

        return id;
    }

    private void ChangeScript(TMP_Text tmpText, string script)
    {
        tmpText.text = script;
    }

    private void OpenUI()
    {
        _panel.SetActive(true);
    }

    public void CloseUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _panel.SetActive(false);
    }

    public Button GetButton(int buttonId)
    {
        foreach (var button in _choiceButtons)
        {
            if (button.ID == buttonId)
                return button.Button;
        }

        return null;
    }
}
