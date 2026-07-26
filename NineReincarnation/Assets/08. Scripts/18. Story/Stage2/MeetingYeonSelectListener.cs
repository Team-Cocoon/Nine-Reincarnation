using ExcelData;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MeetingYeonSelectListener : SelectUIButtonListener
{
    [Inject] private DialogueDB _dialogueDB;

    [SerializeField] private int _redThreadId = 201017;
    [SerializeField] private int _redThreadEndID = 201018;
    private bool _isRedThreadClicked = false;
    
    [SerializeField] private int _blueThreadId = 201018;
    [SerializeField] private int _blueThreadEndID = 201020;
    private bool _isBlueThreadClicked = false;

    [SerializeField] private int _nextDialogueID = 201021;

    private Button _buttonTemp;

    public override void ConnectSelectUI(SelectUI selectUI)
    {
        base.ConnectSelectUI(selectUI);

        _isRedThreadClicked = _isBlueThreadClicked = false;
    }

    public override int OnButtonClicked(int id)
    {
        base.OnButtonClicked(id);

        if (_isBlueThreadClicked == true && _isRedThreadClicked == true)
            return _nextDialogueID;

        _buttonTemp = GetSelectButton(id);
        if (_buttonTemp != null)
            _buttonTemp.interactable = false;


        if (_redThreadId == id)
        {
            _isRedThreadClicked = true;
            _dialogueDB.GetData<DialogueClass>(_blueThreadEndID).NextID = _nextDialogueID;
        }

        else if (_blueThreadId == id)
        {
            _isBlueThreadClicked = true;
            _dialogueDB.GetData<DialogueClass>(_redThreadEndID).NextID = _nextDialogueID;
        }

        return id;
    }
}
