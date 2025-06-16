using UnityEngine;

public class MainUI : MonoBehaviour
{
    [Header("--- 캔버스 목록 ---")]
    [SerializeField] private GameObject _listSelectUI;
    [SerializeField] private GameObject _listUI;
    [SerializeField] private GameObject _profileUI;
    [SerializeField] private GameObject _infoUI;
    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        UIEventHandler.OnOpenListSeclectUI += OpenListSelectUI;
        UIEventHandler.OnOpenListUI += OpenListUI;
        UIEventHandler.OnOpenInfoUI += OpenInfoUI;
        UIEventHandler.OnOpenProfileUI += OpenProfileUI;
        UIEventHandler.OnCloseMainUI += CloseMainUI;

        Init();
    }

    private void OnDisable()
    {
        UIEventHandler.OnOpenListSeclectUI -= OpenListSelectUI;
        UIEventHandler.OnOpenListUI -= OpenListUI;
        UIEventHandler.OnOpenInfoUI -= OpenInfoUI;
        UIEventHandler.OnOpenProfileUI -= OpenProfileUI;
        UIEventHandler.OnCloseMainUI -= CloseMainUI;
    }

    private void OnDestroy()
    {
        UIEventHandler.OnOpenListSeclectUI -= OpenListSelectUI;
        UIEventHandler.OnOpenListUI -= OpenListUI;
        UIEventHandler.OnOpenInfoUI -= OpenInfoUI;
        UIEventHandler.OnOpenProfileUI -= OpenProfileUI;
        UIEventHandler.OnCloseMainUI -= CloseMainUI;
    }

    private void Init()
    {
        _profileUI.SetActive(false);
        _listSelectUI.SetActive(false);
        _listUI.SetActive(false);
        _infoUI.SetActive(false);
    }

    private void CloseMainUI()
    {
        Init();
    }

    private void OpenListSelectUI()
    {
        Init();
        _listSelectUI.SetActive(true);
    }

    private void OpenListUI()
    {
        Init();
        _listUI.SetActive(true);
    }
    private void OpenProfileUI()
    {
        _profileUI.SetActive(true);
    }

    private void OpenInfoUI()
    {
        _profileUI.SetActive(false);
        _infoUI.SetActive(true);
    }
}
