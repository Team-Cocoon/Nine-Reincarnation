using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExcelData;
using Febucci.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : GameUI
{
    [Header("----Image-----")]
    [SerializeField] private Sprite[] _sprites;

    [Header("---- UI-----")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TypewriterCore _scriptText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _image;

    private Dictionary<string, Sprite> _spriteDict = new();
    private CancellationToken _token;

    private bool _hasSkipEvent = false;
    public bool HasSkipEvent => _hasSkipEvent;

    private void Awake()
    {
        _token = new();

        foreach (Sprite sprite in _sprites)
        {
            _spriteDict.Add(sprite.name, sprite);
        }

    }

    public async UniTask UpdateUI(ScriptClass scriptData, CancellationToken token)
    {
        CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _token);

        _panel.SetActive(true);

        string name = scriptData.ShowName;
        string imageName = scriptData.ImageName;
        string script = scriptData.Script;

        if (!string.IsNullOrEmpty(name))
        {
            ChangeName(name);
        }

        if (!string.IsNullOrEmpty(imageName))
        {
            ChangeSprite(imageName);
        }

        if (!string.IsNullOrEmpty(script))
        {
            await ChangeScript(script, linkedCts);
        }

        _hasSkipEvent = true;
    }

    public void CloseUI()
    {
        _hasSkipEvent = false;
        _panel.SetActive(false);
    }

    //표정 이미지 변경
    private void ChangeSprite(string name)
    {
        _image.sprite = _spriteDict[name];
    }

    //스크립트 변경
    private async UniTask ChangeScript(string script, CancellationTokenSource cts)
    {
        //var waitTask = _scriptText.onTextShowed.OnInvokeAsync(this.GetCancellationTokenOnDestroy());

        //_scriptText.TextAnimator.textFull = script;

        _scriptText.ShowText(script);

        while (_scriptText.isShowingText)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _scriptText.SkipTypewriter();
            }

            // 다음 프레임까지 대기 (파괴 시 취소 토큰 연결)
            await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
        }
    }

    //화자 변경
    private void ChangeName(string name)
    {
        _nameText.text = name;
    }
}
