using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    public async UniTask UpdateUI(ScriptClass scriptData)
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
            ChangeSprite(imageName);
        }

        if (!string.IsNullOrEmpty(script))
        {
            await ChangeScript(script);
        }
    }

    //표정 이미지 변경
    private void ChangeSprite(string name)
    {
        _image.sprite = _spriteDict[name];
    }

    //스크립트 변경
    private async UniTask ChangeScript(string script)
    {
        var waitTask = _scriptText.onTextShowed.OnInvokeAsync(this.GetCancellationTokenOnDestroy());

        _scriptText.TextAnimator.textFull = script;

        await waitTask;
    }

    //화자 변경
    private void ChangeName(string name)
    {
        _nameText.text = name;
    }
}
