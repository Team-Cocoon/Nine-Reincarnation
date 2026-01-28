using TMPro;
using UnityEngine;

public class BubbleSizer : MonoBehaviour
{
    [Header("--- 말풍선 최소, 최대 너비 ---")]
    [SerializeField] float _minWidth;
    [SerializeField] float _maxWidth;

    [Header("--- 텍스트를 넣어 주세요 ---")]
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private RectTransform rt;

    private void OnValidate()
    {
        ResizeBubble(tmp.text);
    }

    public void ResizeBubble(string text)
    {
        Vector2 expectedSize = tmp.GetPreferredValues(text); //예상 사이즈 계산

        float finalWidth = Mathf.Max(Mathf.Min(expectedSize.x, _maxWidth), _minWidth);
        rt.sizeDelta = new Vector2(finalWidth, rt.sizeDelta.y);
    }
}
