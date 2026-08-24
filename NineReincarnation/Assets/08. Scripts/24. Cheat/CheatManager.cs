using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VContainer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class CheatManager : MonoBehaviour
{
    [Inject]
    private StageManager _stageManager;

    public UnityEvent<CheatInfo> OnCheat { get; private set; } = new UnityEvent<CheatInfo>();

    public bool IsCheatOn => _isCheatOn;
    [SerializeField] private bool _isCheatOn = false;

    [Header("--- UI ---")]
    [SerializeField] private GameObject _cheatUI;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _button;
    private bool _isCheatUIOpened = false;

    private bool _isMoving = false;
    private CheatInfo _cheatInfo = null;
    public CheatInfo GetCheatInfo { get => _cheatInfo; }

    [Header("--- Data ---")]
    [SerializeField] private CheatDataSO _data;

    private void Awake()
    {
        ResetUI();

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnBtnClicked);
    }

    private void Update()
    {
        // 어차피 disable 하면 안돌아가니까 update에서 인풋 처리
        if((Input.GetKey(KeyCode.LeftBracket) && Input.GetKeyDown(KeyCode.RightBracket)) ||
            (Input.GetKeyDown(KeyCode.LeftBracket) && Input.GetKey(KeyCode.RightBracket)))
        {
            OpenCheatUI(true);
        }
    }

    private void ResetUI()
    {
        List<string> options = new List<string>();
        options = _data.CheatInfoList.Select(d => d.OptionName).ToList();

        _dropdown.ClearOptions();
        _dropdown.AddOptions(options);
    }

    private void OpenCheatUI(bool isOpen)
    {
        _isCheatUIOpened = isOpen;
        _cheatUI.SetActive(isOpen);
    }

    private void OnBtnClicked()
    {
        MoveToPoint().Forget();
    }

    private async UniTaskVoid MoveToPoint()
    {
        _isMoving = true;
        int index = _dropdown.value;
        _cheatInfo = _data.CheatInfoList[index];

        bool isChanged = await _stageManager.GoToMap(_cheatInfo.StageNumber, _cheatInfo.SubSceneNumber);
        if (isChanged)
        {
            OpenCheatUI(false);
            OnCheat?.Invoke(_cheatInfo);
        }
        else
        {
            _isMoving = false;
        }
    }

    public bool IsMapMovedByCheat() => _isCheatOn && _isMoving;

    public void DoneCheatMoving()
    {
        if (IsMapMovedByCheat() == false)
            return;

        _isMoving = false;
        _cheatInfo = null;
    }
}
