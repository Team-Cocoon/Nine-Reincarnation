using NUnit.Framework;
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

    private void OnValidate()
    {
        ResizeBubble(tmp.text, (int) _type);
    }

    public void ResizeBubble(string text, int type)
    {
        if (_type < 0) _type = 0;

        Vector2 expectedSize = tmp.GetPreferredValues(text); //예상 사이즈 계산

        float finalWidth = Mathf.Max(Mathf.Min(expectedSize.x, _maxWidth), _minWidthByType[type]);
        rt.sizeDelta = new Vector2(finalWidth, rt.sizeDelta.y);

        _image.pixelsPerUnitMultiplier = Mathf.Lerp(_minMultiplier, _maxMultiplier, 
            (expectedSize.y - _minHeight) / (_maxHeight - _minHeight));
    }
}
