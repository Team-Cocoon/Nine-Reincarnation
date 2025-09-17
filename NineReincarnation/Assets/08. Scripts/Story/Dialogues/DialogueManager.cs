using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [Header("대화창에 뜰 이미지")]
    public List<Sprite> images;

    public bool isTextShowed = false;

    [Header("데이터 파싱을 통해 얻어온 정보")]
    [SerializeField] private TextMeshProUGUI _tmpText;
    [SerializeField] private TextMeshProUGUI _tmpName;
    [SerializeField] private Febucci.UI.Core.TypewriterCore _typeWriter;
    [SerializeField] private GameObject _windows;
    [SerializeField] private Image _image;

    /* 기본 폰트 정보 */
    private Color _initFontColor;
    private float _initFontSize;

    private Dictionary<string, Sprite> _imageDict = new();

    public Febucci.UI.Core.TypewriterCore TypeWriter => _typeWriter;
    public Color InitFontColor => _initFontColor;
    public float InitFontSize => _initFontSize;

    public void Awake()
    {
        Instance = this;
        if (images != null)
        {
            foreach (var image in images)
            {
                _imageDict[image.name] = image;
            }
        }
        _initFontColor = _tmpText.color;
        _initFontSize = _tmpText.fontSize;
    }

    public void StartDialogue()
    {
        ShowWindow(true);
    }
    public bool EndDialogue()
    {
        if (isTextShowed && Input.GetMouseButtonDown(0))
        {
            ShowWindow(false);
            _typeWriter.TextAnimator.textFull = "";
            SetFontColor(InitFontColor);
            SetFontSize(InitFontSize);
            return true;
        }
        return false;
    }
    public void ShowWindow(bool show)
    {
        _windows.SetActive(show);
    }
    public void SeTextData(string contextsData) // 대사 적용
    {
        isTextShowed = false;
        contextsData = contextsData.Replace("*", "\n");
        contextsData = contextsData.Replace("#", ",");
        _typeWriter.TextAnimator.textFull = contextsData;
    }

    public void SetNameData(string nameData)
    {
        _tmpName.text = nameData;
    }

    public void SetExpressionData(string expressionData)
    {
        if (expressionData == "")
        {
            return;
        }
        _image.sprite = _imageDict[expressionData];
    }
    public void TextShowed()
    {
        isTextShowed = true;
        _tmpText.fontSize = 25;
    }

    public void SetFontColor(Color color)
    {
        _tmpText.color = color;
    }

    public void SetFontSize(float size)
    {
        _tmpText.fontSize = size;
    }

    public void PlayTextSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Text);
    }
}
