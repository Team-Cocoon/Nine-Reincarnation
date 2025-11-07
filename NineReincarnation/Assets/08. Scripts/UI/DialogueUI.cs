using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ExcelData;
using Febucci.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("----Image-----")]
    [SerializeField] private Sprite[] _sprites;

    [Header("---- UI-----")]
    [SerializeField] private TypewriterCore _scriptText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _image;

    private Dictionary<string, Sprite> _spriteDict = new();

    private void Awake()
    {
        foreach(Sprite sprite in _sprites)
        {
            _spriteDict.Add(sprite.name, sprite);
        }
    }

    public void UpdateUI(ScriptClass scriptData)
    {
        string name = scriptData.Name;
        string imageName = scriptData.ImageName;
        string script = scriptData.Script;

        if (!string.IsNullOrEmpty(name))
        {
            ChangeSprite(name);
        }

        if (!string.IsNullOrEmpty(imageName))
        {
            ChangeScript(imageName);
        }

        if (!string.IsNullOrEmpty(script))
        {
            ChangeName(script);
        }
    }

    //표정 이미지 변경
    private void ChangeSprite(string name)
    {
        _image.sprite = _spriteDict[name];
    }
    //스크립트 변경
    private void ChangeScript(string script)
    {
        _scriptText.TextAnimator.textFull = script;
    }
    //화자 변경
    private void ChangeName(string name)
    {
        _nameText.text = name;
    }
}
