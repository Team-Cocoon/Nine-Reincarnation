using Cysharp.Threading.Tasks;
using ExcelData;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.UI;

public class BubbleUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private BubbleDataSO _bubbleImageData;
    [SerializeField] private Image _image;
    [SerializeField] private TypewriterCore _scriptText;
    [SerializeField] private BubbleSizer _sizer;
    [SerializeField] private BubbleImageType _currentType = BubbleImageType.None;

    public async UniTask UpdateUI(BubbleClass bubbleData)
    {
        OpenUI();

        if (_currentType != bubbleData.Type)
        {
            SetImage(bubbleData.Type);
            _currentType = bubbleData.Type;
        }

        if (!string.IsNullOrEmpty(bubbleData.Script))
        {
            await ChangeScript(bubbleData.Script);
        }
    }


    private async UniTask ChangeScript(string script)
    {
        _sizer.ResizeBubble(script, (int)_currentType);

        _scriptText.ShowText(script);

        while (_scriptText.isShowingText)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _scriptText.SkipTypewriter();
            }

            // 다음 프레임까지 대기 (파괴 시 취소 토큰 연결)
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }
    }

    private void SetImage(BubbleImageType type)
    {
        _image.sprite = _bubbleImageData.GetSprite((int)type);
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
