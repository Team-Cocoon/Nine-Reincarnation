using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public List<Sprite> images;

    public bool isTextShowed = false;

    [Header("데이터 파싱을 통해 얻어온 정보")]
    [SerializeField] private TextMeshProUGUI _tmpText;
    [SerializeField] private Febucci.UI.Core.TypewriterCore _typeWriter;
    [SerializeField] private GameObject _windows;
    [SerializeField] private Image _image;

    private Dictionary<string, Sprite> _imageDict = new();
    public void Awake()
    {
        Instance = this;
        if(images != null)
        {
            foreach (var image in images)
            {
                _imageDict[image.name] = image;
            }
        }
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
        contextsData = contextsData.Replace("*", "\n");
        _typeWriter.TextAnimator.textFull = contextsData;
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
    }
}
