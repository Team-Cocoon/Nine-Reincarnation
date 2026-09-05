using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LoadingUIStarter : MonoBehaviour
{
    [Header("기존 타이틀 로딩")]
    [SerializeField] private GameObject _loadingContent;
    [Header("문 (이름이 아닌 화면에 배치된 방향 기준)")]
    [SerializeField] private RectTransform _screenLeftDoor;
    [SerializeField] private RectTransform _screenRightDoor;
    [Header("도트 문 연출")]
    [SerializeField, Min(0.01f)] private float _moveDuration = 0.375f;
    [SerializeField, Min(1)] private int _animationFrameRate = 24;
    [SerializeField, Min(0f)] private float _closedHoldDuration = 0.083333f;
    [Header("닫힘 충돌감")]
    [Tooltip("문 한쪽의 반동 거리. 화면 픽셀이 아닌 원화 픽셀 기준이며 0이면 반동을 끕니다.")]
    [SerializeField, Range(0, 3)] private int _closeReboundPixels = 2;
    [Tooltip("맞부딪힌 순간 멈출 애니메이션 프레임 수")]
    [SerializeField, Range(0, 2)] private int _impactHoldFrames = 1;

    private Image _background;
    private Color _backgroundColor;
    private bool _backgroundRaycast;
    private bool[] _indicatorStates;
    private bool _initialized;
    private bool _doorMode;
    private float _closure;
    private int _reboundPixels;
    private Rect _lastRootRect;
    private RectTransform _root;
    private DoorLayout _left;
    private DoorLayout _right;

    public bool HasDoors => _screenLeftDoor != null && _screenRightDoor != null;

    private sealed class DoorLayout
    {
        public RectTransform Rect;
        public Vector2 OriginalPosition;
        public Vector2 OriginalSize;
        public float Aspect;
        public float SpriteWidth;
        public float Direction;
    }

    private void Awake()
    {
        Initialize();
        SetDoorsActive(false);
    }

    private void Initialize()
    {
        if (_initialized) return;
        _root = (RectTransform)transform;
        if (_loadingContent == null) _loadingContent = transform.Find("Image")?.gameObject;
        _background = _loadingContent != null ? _loadingContent.GetComponent<Image>() : null;
        if (_background != null)
        {
            _backgroundColor = _background.color;
            _backgroundRaycast = _background.raycastTarget;
        }
        if (_loadingContent != null)
        {
            _indicatorStates = new bool[_loadingContent.transform.childCount];
            for (int i = 0; i < _indicatorStates.Length; i++)
                _indicatorStates[i] = _loadingContent.transform.GetChild(i).gameObject.activeSelf;
        }
        if (HasDoors)
        {
            _left = CaptureDoor(_screenLeftDoor, -1f);
            _right = CaptureDoor(_screenRightDoor, 1f);
        }
        _initialized = true;
    }

    private static DoorLayout CaptureDoor(RectTransform door, float direction)
    {
        var doorImage = door.GetComponent<Image>();
        var sprite = doorImage != null ? doorImage.sprite : null;
        return new DoorLayout
        {
            Rect = door,
            OriginalPosition = door.anchoredPosition,
            OriginalSize = door.sizeDelta,
            Aspect = sprite != null ? sprite.rect.width / sprite.rect.height : door.rect.width / door.rect.height,
            SpriteWidth = sprite != null ? sprite.rect.width : 480f,
            Direction = direction
        };
    }

    public async UniTask ShowAsync(bool useDoors, CancellationToken token)
    {
        Initialize();
        _doorMode = useDoors && HasDoors;
        gameObject.SetActive(true);
        SetDoorsActive(false);
        ConfigureContent(_doorMode);
        if (!_doorMode) return;

        Canvas.ForceUpdateCanvases();
        _reboundPixels = 0;
        SetClosure(0f);
        SetDoorsActive(true);
        await AnimateAsync(1f, token);
        await PlayCloseReboundAsync(token);
        // Render the closed pose before scene activation can stall a frame.
        await UniTask.Delay(TimeSpan.FromSeconds(_closedHoldDuration), ignoreTimeScale: true,
            cancellationToken: token);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
    }

    public async UniTask HideAsync(CancellationToken token)
    {
        try
        {
            if (_doorMode && gameObject.activeInHierarchy)
                await AnimateAsync(0f, token);
        }
        finally
        {
            if (this != null) HideImmediate();
        }
    }

    public void HideImmediate()
    {
        Initialize();
        _doorMode = false;
        _reboundPixels = 0;
        SetDoorsActive(false);
        RestoreDoor(_left);
        RestoreDoor(_right);
        ConfigureContent(false);
        gameObject.SetActive(false);
    }

    private async UniTask PlayCloseReboundAsync(CancellationToken token)
    {
        int pixels = Mathf.Clamp(_closeReboundPixels, 0, 3);
        if (pixels == 0) return;

        double frameSeconds = 1.0 / Mathf.Max(1, _animationFrameRate);
        try
        {
            // Hold the contact pose for one animation frame to emphasize impact.
            await UniTask.Delay(TimeSpan.FromSeconds(frameSeconds * Mathf.Clamp(_impactHoldFrames, 0, 2)),
                ignoreTimeScale: true, cancellationToken: token);

            SetReboundPixels(pixels);
            await UniTask.Delay(TimeSpan.FromSeconds(frameSeconds), ignoreTimeScale: true,
                cancellationToken: token);
            SetReboundPixels(pixels / 2);
            await UniTask.Delay(TimeSpan.FromSeconds(frameSeconds), ignoreTimeScale: true,
                cancellationToken: token);
        }
        finally
        {
            // Loading may start only after the seam is sealed again. Also clear
            // the transient offset if cancellation interrupts either rebound pose.
            if (this != null && HasDoors) SetReboundPixels(0);
        }
    }

    private void SetReboundPixels(int pixels)
    {
        _reboundPixels = pixels;
        SetClosure(_closure);
    }

    private async UniTask AnimateAsync(float target, CancellationToken token)
    {
        float start = _closure;
        float duration = Mathf.Max(0.01f, _moveDuration) * Mathf.Abs(target - start);
        if (duration <= 0f) return;
        int frames = Mathf.Max(1, Mathf.RoundToInt(duration * Mathf.Max(1, _animationFrameRate)));
        double frameDuration = (double)duration / frames;
        double pendingTime = 0.0;
        int step = 0;
        var clock = System.Diagnostics.Stopwatch.StartNew();
        double previousTime = clock.Elapsed.TotalSeconds;
        while (step < frames)
        {
            token.ThrowIfCancellationRequested();
            float steppedTime = (float)step / frames;
            SetClosure(Mathf.Lerp(start, target, steppedTime));

            // Yield(Update) may resume in the same frame. A long loading frame's
            // unscaledDeltaTime could then consume the entire opening animation.
            // Measure only time since this animation started and present each step
            // on a distinct frame, slowing down instead of skipping poses on a hitch.
            await UniTask.NextFrame(PlayerLoopTiming.Update, token);
            double now = clock.Elapsed.TotalSeconds;
            pendingTime += Math.Min(Math.Max(0.0, now - previousTime), frameDuration);
            previousTime = now;
            if (pendingTime >= frameDuration)
            {
                pendingTime -= frameDuration;
                step++;
            }
        }
        SetClosure(target);
        // Keep the final pose visible for a frame before HideImmediate disables UI.
        await UniTask.NextFrame(PlayerLoopTiming.Update, token);
    }

    private void SetClosure(float closure)
    {
        _closure = closure;
        _lastRootRect = _root.rect;
        // Main travel reverses exactly when opening; the contact rebound is a
        // separate closing-only beat, not an elastic ease on the whole movement.
        float travel = closure < 0.8f ? closure * 1.125f : 0.9f + (closure - 0.8f) * 0.5f;
        PositionDoor(_left, _lastRootRect, travel, _reboundPixels);
        PositionDoor(_right, _lastRootRect, travel, _reboundPixels);
    }

    private static void PositionDoor(DoorLayout door, Rect rootRect, float travel, int reboundPixels)
    {
        // Uniform scaling covers any aspect ratio by cropping outside the viewport.
        float width = Mathf.Max(rootRect.width * 0.5f, rootRect.height * door.Aspect);
        float height = width / door.Aspect;
        float pixelStep = width / Mathf.Max(1f, door.SpriteWidth);
        float offset = Mathf.Round((rootRect.width * 0.5f + pixelStep) * (1f - travel) / pixelStep) * pixelStep;
        offset += reboundPixels * pixelStep;
        float closedX = rootRect.center.x + (door.Direction < 0f
            ? -width * (1f - door.Rect.pivot.x) : width * door.Rect.pivot.x);
        Vector2 anchor = new Vector2(
            Mathf.Lerp(door.Rect.anchorMin.x, door.Rect.anchorMax.x, door.Rect.pivot.x),
            Mathf.Lerp(door.Rect.anchorMin.y, door.Rect.anchorMax.y, door.Rect.pivot.y));
        Vector2 anchorPosition = rootRect.min + Vector2.Scale(rootRect.size, anchor);
        door.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        door.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        door.Rect.anchoredPosition = new Vector2(closedX + door.Direction * offset,
            rootRect.center.y + (door.Rect.pivot.y - 0.5f) * height) - anchorPosition;
    }

    private void LateUpdate()
    {
        if (_doorMode && _root.rect != _lastRootRect) SetClosure(_closure);
    }

    private void ConfigureContent(bool doors)
    {
        if (_loadingContent == null) return;
        _loadingContent.SetActive(true);
        if (_background != null)
        {
            // Keep a transparent full-screen graphic to block UI clicks in gaps.
            _background.color = doors ? new Color(0f, 0f, 0f, 0f) : _backgroundColor;
            _background.raycastTarget = doors || _backgroundRaycast;
        }
        for (int i = 0; i < _indicatorStates.Length; i++)
            _loadingContent.transform.GetChild(i).gameObject.SetActive(!doors && _indicatorStates[i]);
    }

    private void SetDoorsActive(bool active)
    {
        if (_screenLeftDoor != null) _screenLeftDoor.gameObject.SetActive(active);
        if (_screenRightDoor != null) _screenRightDoor.gameObject.SetActive(active);
    }

    private static void RestoreDoor(DoorLayout door)
    {
        if (door == null || door.Rect == null) return;
        door.Rect.anchoredPosition = door.OriginalPosition;
        door.Rect.sizeDelta = door.OriginalSize;
    }
}
