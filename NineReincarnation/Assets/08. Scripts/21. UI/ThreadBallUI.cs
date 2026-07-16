using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

public class ThreadBallUI : MonoBehaviour, IClickInteractableToggle
{
    [SerializeField] private InputConnector _inputConnector;

    [SerializeField] private GameObject _panel;
    [SerializeField] private GraphicRaycaster _raycaster;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _openTime = 2f;
    [SerializeField] private float _closeTime = 1f;

    [SerializeField] private UnityEvent _onUIClosed;

    public bool IsClickControlToSelf => false;
    private bool _isClick = false;
    private bool _isActive = false;

    private void Awake()
    {
        if(_inputConnector == null)
            _inputConnector = GetComponent<InputConnector>();
    }

    private void OnEnable()
    {
        _raycaster.enabled = false;
    }

    public async UniTask OpenUI(CancellationToken token)
    {
        _inputConnector?.InputManager?.ChangeActionToUI();
        _panel.SetActive(true);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
        token,
        this.GetCancellationTokenOnDestroy());
        await _canvasGroup.DOFade(1f, _openTime).ToUniTask(cancellationToken: linkedCts.Token);

        _raycaster.enabled = true;
        _isActive = true;
    }

    public async UniTask CloseUI()
    {
        await _canvasGroup.DOFade(0f, _closeTime).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        _panel.SetActive(false);
        _isClick = true;
        _raycaster.enabled = false;
        _isActive = false;
        _onUIClosed?.Invoke();
        _inputConnector?.InputManager?.ChangeActionToPlayer();
    }

    public async UniTask ExecuteEvent(int index)
    {
        await UniTask.WaitUntil(() => _isClick == true, cancellationToken: this.destroyCancellationToken);
    }

    public void EnableClickInteraction()
    {
        if (_isActive == true) CloseUI().Forget();
    }

    public void DisableClickInteraction()
    {

    }
}
