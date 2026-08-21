using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ChaseGhostUI : MonoBehaviour, IEventInterface, IClickInteractableToggle
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsClickControlToSelf => false;
    private bool _isClick = false;
    private bool _isActive = false;
    public async UniTask OpenUI(CancellationToken token)
    {
        _panel.SetActive(true);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
        token,
        this.GetCancellationTokenOnDestroy());
        await _canvasGroup.DOFade(1f, 2f).ToUniTask(cancellationToken: linkedCts.Token);

        _isActive = true;
    }

    public async UniTask CloseUI()
    {
        await _canvasGroup.DOFade(0f, 2f).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        _panel.SetActive(false);
        _isClick = true;
        _isActive = false;
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
