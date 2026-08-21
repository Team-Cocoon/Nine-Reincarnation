using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PhasableObject : DrawOutline, IThreadInteractable, IPhasable
{
    [SerializeField] private float _targetAlpha = 0.15f;
    private SpriteRenderer _spriteRenderer;
    private Tween _phaseTween;
    private int _phaseVersion;
    private bool _phasingCounted;
    private Player.Controller.PlayerController _phasingPlayer;

    [Header("----- 통과(막힘) 처리 -----")]
    [SerializeField] private string _solidLayerName = "Obstacle";
    private Collider2D _solidBlocker;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activatedColor;

    [Header("----- 상호작용 조절 -----")]
    [SerializeField] private bool _possibleActive;
    [SerializeField] private bool _isActivated;
    [SerializeField] private bool _isClickControlToSelf;
    [SerializeField] private bool _isHoverControlToSelf;
    [SerializeField] private float _interactionDistance;
    [SerializeField] private ThreadCompatibility _allowedThreads = ThreadCompatibility.Blue;

    private Collider2D _collider2D;

    private Vector2 _playerPosition
    {
        get
        {
            var inputManager = InputManager.Instance;
            return inputManager != null && inputManager.CurPlayer != null
                ? inputManager.CurPlayer.transform.position
                : transform.position;
        }
    }

    public ThreadCompatibility AllowedThreads => _allowedThreads;
    public bool IsClickControlToSelf => _isClickControlToSelf;
    public override bool IsHoverControlToSelf => _isHoverControlToSelf;
    public bool IsConnected => _isActivated;

    protected override void Awake()
    {
        base.Awake();
        _possibleActive = false;
        _isActivated = false;
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        CreateSolidBlocker();
    }

    // 플레이어 이동만 막는 전용 자식 콜라이더를 만든다.
    private void CreateSolidBlocker()
    {
        int solidLayer = LayerMask.NameToLayer(_solidLayerName);
        if (solidLayer < 0)
        {
            Debug.LogWarning(
                $"[PhasableObject] '{_solidLayerName}' 레이어를 찾을 수 없어 막힘 콜라이더를 생성하지 못했습니다.");
            return;
        }

        GameObject blocker = new GameObject("PhaseSolidBlocker");
        blocker.transform.SetParent(transform, false);
        blocker.layer = solidLayer;

        BoxCollider2D box = blocker.AddComponent<BoxCollider2D>();
        if (_collider2D is BoxCollider2D srcBox)
        {
            box.size = srcBox.size;
            box.offset = srcBox.offset;
        }
        else if (_collider2D != null)
        {
            box.size = _collider2D.bounds.size;
            box.offset = transform.InverseTransformPoint(_collider2D.bounds.center);
        }

        _solidBlocker = box;
        SetSolid(true);
    }

    protected virtual void SetSolid(bool solid)
    {
        if (_solidBlocker != null)
        {
            _solidBlocker.enabled = solid;
        }
    }

    private void Update()
    {
        if (!IsOutline) return;

        float distance = Vector2.Distance(_playerPosition, transform.position);
        if (distance > _interactionDistance)
        {
            if (!_possibleActive) return;

            OutlineColor = _inactiveColor;
            _possibleActive = false;

            if (_isActivated)
            {
                ForceDisconnect();
                IsOutline = false;
            }
        }
        else
        {
            if (_isActivated || _possibleActive) return;

            OutlineColor = _activeColor;
            _possibleActive = true;
        }
    }

    public void PhaseIn()
    {
        // 버전을 먼저 올려 이전 지연 작업을 무효화한다. 수동 CancellationToken을
        // 취소하지 않으므로 DOTween/UniTask 콜백이 ForceDisconnect 안으로 재진입하지 않는다.
        int phaseVersion = ++_phaseVersion;
        ReleaseActivePhasing();
        KillPhaseTween();

        OutlineColor = _activatedColor;
        _isActivated = true;
        SetSolid(false);
        Debug.Log("청연 연결 시작");

        var inputManager = InputManager.Instance;
        _phasingPlayer = inputManager != null ? inputManager.CurPlayer : null;
        if (_phasingPlayer != null)
        {
            _phasingPlayer.AddActivePhasing();
            _phasingCounted = true;
        }

        if (_spriteRenderer != null)
        {
            _phaseTween = _spriteRenderer
                .DOFade(_targetAlpha, 0.5f)
                .SetLink(gameObject);
        }

        ExecutePhaseSequence(phaseVersion, this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid ExecutePhaseSequence(int phaseVersion, CancellationToken destroyToken)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: destroyToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (!IsCurrentPhase(phaseVersion)) return;

        ReleaseActivePhasing();
        KillPhaseTween();

        if (_spriteRenderer == null)
        {
            CompleteNaturalPhase(phaseVersion);
            return;
        }

        _phaseTween = _spriteRenderer
            .DOFade(1f, 1f)
            .SetLink(gameObject)
            .OnComplete(() => CompleteNaturalPhase(phaseVersion));
    }

    public void ForceDisconnect()
    {
        if (!_isActivated && !_phasingCounted) return;

        // Kill보다 버전 무효화를 먼저 해 이전 비동기 흐름이 상태를 다시 건드리지 못하게 한다.
        ++_phaseVersion;
        ReleaseActivePhasing();
        RestoreDisconnectedState();
    }

    public void PhaseOut()
    {
        ForceDisconnect();
    }

    private bool IsCurrentPhase(int phaseVersion)
    {
        return _isActivated && phaseVersion == _phaseVersion;
    }

    private void CompleteNaturalPhase(int phaseVersion)
    {
        if (!IsCurrentPhase(phaseVersion)) return;

        // 완료 콜백 안에서 같은 트윈을 다시 Kill하지 않도록 참조부터 비운다.
        _phaseTween = null;
        RestoreDisconnectedState();
    }

    private void RestoreDisconnectedState()
    {
        KillPhaseTween();
        OutlineColor = _activeColor;
        _isActivated = false;
        SetSolid(true);

        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
        }

        Debug.Log("청연 연결 끝");
    }

    private void ReleaseActivePhasing()
    {
        if (_phasingCounted && _phasingPlayer != null)
        {
            _phasingPlayer.RemoveActivePhasing();
        }

        _phasingCounted = false;
        _phasingPlayer = null;
    }

    private void KillPhaseTween()
    {
        if (_phaseTween == null) return;

        _phaseTween.Kill(false);
        _phaseTween = null;
    }

    private void OnDestroy()
    {
        ++_phaseVersion;
        ReleaseActivePhasing();
        KillPhaseTween();
    }

    public void OnThreadHit(ThreadType threadType)
    {
        switch (threadType)
        {
            case ThreadType.Red:
                break;
            case ThreadType.Blue:
                EnableClickInteraction();
                break;
        }
    }

    public void EnableClickInteraction()
    {
        PhaseIn();
    }

    public override void EnableHoverInteraction()
    {
        if (_isActivated) return;

        OutlineColor = _possibleActive ? _activeColor : _inactiveColor;
        base.EnableHoverInteraction();
    }

    public void DisableClickInteraction()
    {
    }

    public override void DisableHoverInteraction()
    {
        if (_isActivated) return;

        base.DisableHoverInteraction();
    }
}
