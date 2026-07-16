using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThreadBall : FloatingItem
{
    [Header("----ThreadBallUI----")]
    [SerializeField] private ThreadBallUI _ballUI;

    public override void OnAcquired(Collider2D collision)
    {
        _ballUI.OpenUI(this.GetCancellationTokenOnDestroy()).Forget();
    }
}
