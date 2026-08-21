using NUnit.Framework;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSizer : MonoBehaviour
{
    [Header("--- 말풍선 최소, 최대 너비 ---")]
    [SerializeField] private float[] _minWidthByType;
    [SerializeField] float _maxWidth;

    [SerializeField] private float _minHeight;
    [SerializeField] private float _maxHeight;
     
    [Header("--- 텍스트를 넣어 주세요 ---")]
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private RectTransform rt;

    [Header("--- BG Slice Multiplier ---")]
    [SerializeField] private Image _image;
    [SerializeField] private float _minMultiplier;
    [SerializeField] private float _maxMultiplier;

    [Header("--- Debug Type ---")]
    [SerializeField] private BubbleImageType _type = BubbleImageType.Normal;

    // 태그 빼기
    private string GetSizeCalculationText(string text)
    {
        text = Regex.Replace(text, @"\{[^}]+\}", "");

        text = Regex.Replace(text, @"<bounce[^>]*>", "");
        text = Regex.Replace(text, @"<shake[^>]*>", "");

        if (Regex.IsMatch(text, @"<size=[^>]+>"))
            text = text.Replace("</>", "</size>");

        return text;
    }

    private void OnValidate()
    {
        ResizeBubble(tmp.text, (int) _type);
    }

    public void ResizeBubble(string text, int type)
    {
        if (_type < 0) _type = 0;

        Vector2 expectedSize = tmp.GetPreferredValues(GetSizeCalculationText(text)); //예상 사이즈 계산

        float finalWidth = Mathf.Max(Mathf.Min(expectedSize.x, _maxWidth), _minWidthByType[type]);
        rt.sizeDelta = new Vector2(finalWidth, rt.sizeDelta.y);

        _image.pixelsPerUnitMultiplier = Mathf.Lerp(_minMultiplier, _maxMultiplier, 
            (expectedSize.y - _minHeight) / (_maxHeight - _minHeight));
    }
}
