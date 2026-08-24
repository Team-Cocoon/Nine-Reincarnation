using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class StoryCanvas : MonoBehaviour
{
    [SerializeField] private StoryFadeUIEvent _fadeEvent;
    public StoryFadeUIEvent FadeUIEvent => _fadeEvent;

    [Header("Letterbox")]
    [SerializeField] private RectTransform _topImg;
    [SerializeField] private RectTransform _bottomImg;

    [SerializeField] private float _letterboxHeight = 150f;

    [Header("Animation")]
    [SerializeField, Min(1f)]
    private float _letterboxSpeed = 300f;

    [SerializeField, Min(0f)]
    private float _bounceAmount = 10f;

    private Sequence _sequence;

    private void Awake()
    {
        SetHeight(0f);
    }

    public async UniTask ShowAsync(CancellationToken token = default)
    {
        KillSequence();
        SetHeight(0f);

        float overshootHeight = _letterboxHeight + _bounceAmount;

        _sequence = DOTween.Sequence()
            .Append(CreateHeightTween(0f, overshootHeight))
            .Append(CreateHeightTween(
                overshootHeight,
                _letterboxHeight
            ));

        await WaitSequenceAsync(token);

        SetHeight(_letterboxHeight);
    }

    public async UniTask HideAsync(CancellationToken token = default)
    {
        KillSequence();

        float currentHeight = _topImg.rect.height;
        float overshootHeight = _letterboxHeight + _bounceAmount;

        _sequence = DOTween.Sequence()
            .Append(CreateHeightTween(
                currentHeight,
                overshootHeight
            ))
            .Append(CreateHeightTween(
                overshootHeight,
                0f
            ));

        await WaitSequenceAsync(token);

        SetHeight(0f);
    }

    public void Hide()
    {
        KillSequence();
        SetHeight(0f);
    }

    private Tween CreateHeightTween(
        float startHeight,
        float targetHeight)
    {
        float distance = Mathf.Abs(targetHeight - startHeight);
        float duration = distance / _letterboxSpeed;

        return DOTween.To(
                () => _topImg.rect.height,
                SetHeight,
                targetHeight,
                duration
            )
            .SetEase(Ease.Linear);
    }

    private void SetHeight(float height)
    {
        height = Mathf.Max(0f, height);

        _topImg.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height
        );

        _bottomImg.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height
        );
    }

    private async UniTask WaitSequenceAsync(CancellationToken token)
    {
        Sequence currentSequence = _sequence;

        if (currentSequence == null)
            return;

        try
        {
            await currentSequence
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(token);
        }
        finally
        {
            if (token.IsCancellationRequested &&
                currentSequence.IsActive())
            {
                currentSequence.Kill();
            }

            if (_sequence == currentSequence)
                _sequence = null;
        }
    }

    private void KillSequence()
    {
        if (_sequence != null && _sequence.IsActive())
            _sequence.Kill();

        _sequence = null;
    }

    private void OnDestroy()
    {
        KillSequence();
    }
}