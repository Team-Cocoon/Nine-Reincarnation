using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExcelData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct SelectDataStruct
{
    private int    _id;
    private int    _nextId;
    private string _script;

    public int    Id     => _id;
    public int    NextId => _nextId;
    public string Script => _script;
    
    public void SetSelectDataStruct(int id, int nextId, string script)
    {
        _id     = id;
        _nextId = nextId;
        _script = script; 
    }
}



public class SelectUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text   _questionText;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [SerializeField] private List<GameObject> _choiceButtons;

    private UniTaskCompletionSource<int> _utcs;

    public void UpdateUI(SelectClass selectData, SelectDataStruct[] selectDataes)
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

        if(_panel.transform.childCount < selectData.ChoiceCount)
        {
            for (int i = 0; i < selectData.ChoiceCount; i++)
            {
                int index = i;

                if (!string.IsNullOrEmpty(selectData.Script))
                {
                    GameObject button = Instantiate(_choiceButtonPrefab, _panel.transform);
                    _choiceButtons.Add(button);

                    button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        Debug.Log(selectDataes[index].NextId);
                        _utcs.TrySetResult(selectDataes[index].NextId); // "나 i번째 버튼 눌렸어!" 하고 신호 보냄
                    });

                    ChangeScript(button.GetComponentInChildren<TMP_Text>(), selectDataes[i].Script);
                }
            }
        }

        OpenUI();
    }

    public async UniTask<int> WaitSelect()
    {
        int id = await _utcs.Task;

        CloseUI();

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
        _panel.SetActive(false);
    }
}
