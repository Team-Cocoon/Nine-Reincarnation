using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ToolTipUICanvas : MonoBehaviour
{
    [Header("---- 툴팁 UI 관련 변수 ----")]
    [SerializeField] private GameObject _listUpdateTooltip;
    [SerializeField] private GameObject _lockedListTooltip;
    [SerializeField] private GameObject _lockedProfileTooltip;

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

        _listUpdateTooltip.SetActive(false);
        _lockedListTooltip.SetActive(false);
        _lockedProfileTooltip.SetActive(false);
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

        _lockedListTooltip.SetActive(true);

        CanvasGroup canvasGroup = _lockedListTooltip.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).SetDelay(_delay)
            .OnComplete(() =>
            {
                _isRun = false;
                _lockedListTooltip.SetActive(false);
            });
    }

    private void OpenListUpdateToolTip(Action action = null)
    {
        if (_isRun) return;

        _isRun = true;

        _listUpdateTooltip.SetActive(true);

        CanvasGroup canvasGroup = _listUpdateTooltip.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        canvasGroup.DOFade(0f, _fadeDuration).SetDelay(_delay).SetUpdate(true).OnComplete(() =>
        {
            _isRun = false;
            _listUpdateTooltip.SetActive(false);
            action?.Invoke();
        });
    }

    private void OpenLockedProfileToolTip()
    {
        if (_isRun) return;

        _isRun = true;

        _lockedProfileTooltip.SetActive(true);

        CanvasGroup canvasGroup = _lockedProfileTooltip.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        RectTransform rect = _lockedProfileTooltip.GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence();

        rect.localPosition = _startPosition;

        seq.Join(rect.DOAnchorPos(_endPosition, _moveDuration))
           .Join(canvasGroup.DOFade(0f, _fadeDuration).SetDelay(_moveDuration - _fadeDuration)).SetUpdate(true)
                       .OnComplete(() =>
                       {
                           _isRun = false;
                           _lockedProfileTooltip.SetActive(false);
                       });
    }
}
