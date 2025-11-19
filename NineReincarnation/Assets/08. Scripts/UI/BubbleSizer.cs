using TMPro;
using UnityEngine;

public class BubbleSizer : MonoBehaviour
{
    [Header("--- 대사 ---")]
    [SerializeField] string _line;

    [Header("--- 말풍선 최소, 최대 너비 ---")]
    [SerializeField] float _minWidth;
    [SerializeField] float _maxWidth;

    [Header("--- 텍스트를 넣어 주세요 ---")]
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private RectTransform rt;

    public string Line
    {
        get { return _line; }
        set
        {
            _line = value;
            SetText();
        }
    }

    private void OnValidate()
    {
        SetText();
    }

    void Start()
    {
        SetText();
    }

    private void SetText()
    {
        tmp.text = _line;
        SetWidth();
    }

    private void SetWidth()
    {
        float width = Mathf.Max(Mathf.Min(tmp.preferredWidth, _maxWidth), _minWidth);
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
}
