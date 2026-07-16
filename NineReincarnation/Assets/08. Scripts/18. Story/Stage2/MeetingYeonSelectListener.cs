using UnityEngine;
using UnityEngine.UI;

public class MeetingYeonSelectListener : SelectUIButtonListener
{
    [SerializeField] private int _redThreadId = 201017;
    private bool _isRedThreadClicked = false;
    
    [SerializeField] private int _blueThreadId = 201018;
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
            _buttonTemp.enabled = false;


        if (_redThreadId == id)
        {
            _isRedThreadClicked = true;
        }

        else if (_blueThreadId == id)
        {
            _isBlueThreadClicked = true;
        }

        return _isRedThreadClicked == true &&
            _isBlueThreadClicked == true ?
            _nextDialogueID : id;
    }
}
