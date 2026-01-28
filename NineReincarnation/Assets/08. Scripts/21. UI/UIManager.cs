using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class UIInfo
{
    [Header("---- 초기 등록 UI ----")]

    public bool IsOpen;
    public GameObject UI;

    public UIInfo(bool _IsOpen, GameObject _UI)
    {
        IsOpen = _IsOpen;
        UI = _UI;
    }
}

public class UIManager : MonoBehaviour
{
    public static int OpenUICount = 0;

    public static UIManager Instance { get; set; }

    private Dictionary<string, UIInfo> _uiDict = new Dictionary<string, UIInfo>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddUIDictionary(string name, GameObject go)
    {
        //이미 있으면 리턴
        if (_uiDict.ContainsKey(name)) return;

        _uiDict.Add(name, new UIInfo(go.activeSelf, go));
    }
    public void RemoveUIDictionary(string name)
    {
        //없으면 리턴
        if (!_uiDict.ContainsKey(name)) return;

        _uiDict.Remove(name);
    }

    private void UpdateInputActionMap()
    {
        if (OpenUICount > 0)
        {
            InputManager.Instance?.CurPlayer.SetStop();
            InputEventHandler.OnChangedActionToUI_Invoke();
        }
        else
        {
            InputEventHandler.OnChangedActionToPlayer_Invoke();
        }
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

            OpenUICount--;
            _uiDict[name].IsOpen = false;
            ui.SetActive(false);
        }
        else
        {
            //열어
            _uiDict[name].IsOpen = true;
            ui.SetActive(true);

            OpenUICount++;
            if (isStop)
            {
                Time.timeScale = 0.0f;
            }
        }

        UpdateInputActionMap();
    }
}