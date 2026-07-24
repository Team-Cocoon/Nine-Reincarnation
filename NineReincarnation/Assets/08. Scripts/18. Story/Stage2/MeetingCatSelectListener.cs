using UnityEngine;
using UnityEngine.UI;

public class MeetingCatSelectListener : SelectUIButtonListener
{
    [SerializeField] private int _pettingNum = 101027;
    [SerializeField] private int _clickCount = 3;

    private int _currentCount = 0;

    public override void ConnectSelectUI(SelectUI selectUI)
    {
        base.ConnectSelectUI(selectUI);

        _currentCount = _clickCount;
    }

    public override int OnButtonClicked(int id)
    {
        base.OnButtonClicked(id);

        if (_currentCount <= 0)
            return id;

        if(_pettingNum == id)
        {
            --_currentCount;
            if(_currentCount == 0)
            {
                Button button = GetSelectButton(id);
                if (button == null) return id;
                button.interactable = false;
            }
        }

        return id;
    }
}
