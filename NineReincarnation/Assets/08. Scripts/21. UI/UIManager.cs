using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Dictionary<string, GameObject> _uiDict = new();

    /// <summary>
    /// UI추가
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ui"></param>
    public void AddUIDictionary(string name, GameObject ui)
    {
        //이미 있으면 리턴
        if (_uiDict.ContainsKey(name)) return;

        _uiDict.Add(name, ui);
    }

    /// <summary>
    /// UI제거
    /// </summary>
    /// <param name="name"></param>
    public void RemoveUIDictionary(string name)
    {
        //없으면 리턴
        if (!_uiDict.ContainsKey(name)) return;

        _uiDict.Remove(name);
    }

    /// <summary>
    /// 모든 UI 닫기
    /// </summary>
    /// <param name="name"></param>
    public void CloseAllUI()
    {
        foreach(KeyValuePair<string, GameObject> ui in _uiDict)
        {
            ui.Value.SetActive(false);
        }
    }
}