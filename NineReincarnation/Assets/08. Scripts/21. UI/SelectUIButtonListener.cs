using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectUIButtonListener : MonoBehaviour
{
    [SerializeField] protected UnityEvent<int> _OnButtonClicked;
    
    private SelectUI _selectUI;
    private bool _isConnected = false;

    public virtual void ConnectSelectUI(SelectUI selectUI)
    {
        _isConnected = true;
        _selectUI = selectUI;
    }

    public virtual void DisConnectSelectUI()
    {
        _isConnected = false;
        _selectUI = null;
    }

    public virtual int OnButtonClicked(int id)
    {
        if (_isConnected == false) return -1;

        _OnButtonClicked?.Invoke(id);

        return id;
    }

    public Button GetSelectButton(int id)
    {
        if( _isConnected == false) return null;
        return _selectUI.GetButton(id);
    }

    public void AddButtonClickedListener(UnityAction<int> _action)
    {
        _OnButtonClicked.RemoveListener(_action);
        _OnButtonClicked.AddListener(_action);
    }
}
