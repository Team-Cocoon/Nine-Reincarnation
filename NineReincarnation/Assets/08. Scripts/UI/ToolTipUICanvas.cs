using DG.Tweening;
using TMPro;
using UnityEngine;

public class ToolTipUICanvas : MonoBehaviour
{
    [Header("---- 툴팁 UI 관련 변수 ----")]
    [SerializeField] private GameObject _toolTipUI;
    [SerializeField] private TextMeshProUGUI _toolTipText;
    [SerializeField] private string _listUpdateTooltip;
    [SerializeField] private string _lockedListTooltip;
    [SerializeField] private string _lockedProfileTooltip;

    [Header("---- 툴팁 UI 등장 효과 제어 ----")]
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _endPosition;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _delay;

    private bool _isRun = false;

    private void Awake()
    {
        UIEventHandler.OnOpenListUpdateToolTipUI += OpenListUpdateToolTip;
        UIEventHandler.OnOpenLockedProfileToolTipUI += OpenLockedProfileToolTip;
        UIEventHandler.OnOpenLockedListToolTipUI += OpenLockedListToolTip;

        _toolTipUI.SetActive(false);
    }

    private void OnDestroy()
    {
        UIEventHandler.OnOpenListUpdateToolTipUI -= OpenListUpdateToolTip;
        UIEventHandler.OnOpenLockedProfileToolTipUI -= OpenLockedProfileToolTip;
        UIEventHandler.OnOpenLockedListToolTipUI -= OpenLockedListToolTip;
    }

    private void OpenLockedListToolTip()
    {
        if (_isRun) return;

        _isRun = true;
        _toolTipText.text = _lockedListTooltip;

        _toolTipUI.SetActive(true);

        CanvasGroup canvasGroup = _toolTipUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).SetDelay(_delay)
            .OnComplete(() =>
            {
                _isRun = false;
                _toolTipUI.SetActive(false);
            });
    }

    private void OpenListUpdateToolTip()
    {
        if (_isRun) return;

        _isRun = true;
        _toolTipText.text = _listUpdateTooltip;

        _toolTipUI.SetActive(true);

        CanvasGroup canvasGroup = _toolTipUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        canvasGroup.DOFade(0f, _fadeDuration).SetDelay(_delay).SetUpdate(true).OnComplete(() =>
        {
            _isRun = false;
            _toolTipUI.SetActive(false);
        });
    }

    private void OpenLockedProfileToolTip()
    {
        if (_isRun) return;

        _isRun = true;
        _toolTipText.text = _lockedProfileTooltip;

        _toolTipUI.SetActive(true);

        CanvasGroup canvasGroup = _toolTipUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        RectTransform rect = _toolTipUI.GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence();

        rect.localPosition = _startPosition;

        seq.Join(rect.DOAnchorPos(_endPosition, _moveDuration))
           .Join(canvasGroup.DOFade(0f, _fadeDuration).SetDelay(_moveDuration - _fadeDuration)).SetUpdate(true)
                       .OnComplete(() =>
                       {
                           _isRun = false;
                           _toolTipUI.SetActive(false);
                       });
    }
}
