using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static bool isFinish = false; // 파싱 끝났는지

    [Header("파일 이름")]
    [SerializeField] private string _csvFileName;
    [Header("파싱 객체")]
    [SerializeField] private DialogueParser _dialogueParser;

    private Dictionary<int, Dialogue> _dialogueDict = new Dictionary<int, Dialogue>();
    private int _dialogueIndex = 1;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        isFinish = false;
    }
    private void Awake()
    {
        Instance = this;
        Dialogue[] dialogues = _dialogueParser.Parse(_csvFileName);
        for (int i = 0; i < dialogues.Length; i++)
        {
            _dialogueDict.Add(i + 1, dialogues[i]);
        }
        isFinish = true;
    }

    public Dialogue GetDialogue()
    {
        if (_dialogueIndex <= _dialogueDict.Count)
        {
            return _dialogueDict[_dialogueIndex++];
        }
        else
        {
            return null;
        }
    }

}
