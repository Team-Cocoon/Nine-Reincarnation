using UnityEngine;
using UnityEngine.UI;

public class ProfileCanvas : MonoBehaviour
{
    [Header("--- 프로파일들 ---")]
    [SerializeField] private Button[] _button;
    [SerializeField] private bool[] _unlocked;
    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        Disable();
    }

    private void Init()
    {
        for (int i = 0; i < _unlocked.Length; ++i)
        {
            if (_unlocked[i])
            {
                _button[i].onClick.AddListener(ListUnlock);
            }
            else
            {
                _button[i].onClick.AddListener(ListLock);
            }
        }
    }

    private void Disable()
    {
        for (int i = 0; i < _unlocked.Length; ++i)
        {
            if (_unlocked[i])
            {
                _button[i].onClick.RemoveListener(ListUnlock);
            }
            else
            {
                _button[i].onClick.RemoveListener(ListLock);
            }
        }
    }

    private void ListUnlock()
    {
        UIEventHandler.OnOpenInfoUI();
    }

    private void ListLock()
    {
        UIEventHandler.OnOpenLockedProfileToolTipUI();
    }
}
