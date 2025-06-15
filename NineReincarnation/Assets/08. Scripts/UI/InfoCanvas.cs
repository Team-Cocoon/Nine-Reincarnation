using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [Header("--- 페이지 ---")]
    [SerializeField] private GameObject[] _page;

    private List<Button> _button = new List<Button>();
    private int _curPage = 0;

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
        _curPage = 0;

        Button button;
        for (int i = 0; i < _page.Length; ++i)
        {
            button = _page[i].GetComponentInChildren<Button>();
            button?.onClick.AddListener(NextPage);

            _button.Add(button);

            if (i == 0)
            {
                _page[i].SetActive(true);
            }
            else
            {
                _page[i].SetActive(false);
            }
        }
    }

    private void Disable()
    {
        for (int i = 0; i < _button.Count; ++i)
        {
            _button[0].onClick.RemoveListener(NextPage);
        }

        _button.Clear();
    }

    private void NextPage()
    {
        _page[_curPage].SetActive(false);

        _curPage = (_curPage + 1) % _page.Length;

        _page[_curPage].SetActive(true);
    }

}
