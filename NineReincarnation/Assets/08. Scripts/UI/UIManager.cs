using System.Collections.Generic;
using UnityEngine;

class UIState
{
    public bool IsOpen;
    public GameObject UI;

    public UIState(bool _IsOpen, GameObject _UI)
    {
        IsOpen = _IsOpen;
        UI = _UI;
    }
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    private Dictionary<string, UIState> _uiDict = new Dictionary<string, UIState>();

    public void AddUIDictionary(string name, GameObject go)
    {
        _uiDict.Add(name, new UIState(go.activeSelf, go));
    }
    public void RemoveUIDictionary(string name)
    {
        _uiDict.Remove(name);
    }

    /// <summary>
    /// UI 토글
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isStop">시간을 멈출지 말지 결정하는 변수</param>
    public void ToggleUI(string name, bool isStop)
    {
        bool isOpen = _uiDict[name].IsOpen;
        GameObject ui = _uiDict[name].UI;

        //이미 열려있다면
        if (isOpen)
        {
            //닫어
            if (isStop)
            {
                Time.timeScale = 1.0f;
            }

            _uiDict[name].IsOpen = false;
            ui.SetActive(false);
        }
        else
        {
            //열어
            _uiDict[name].IsOpen = true;
            ui.SetActive(true);

            if (isStop)
            {
                Time.timeScale = 0.0f;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}