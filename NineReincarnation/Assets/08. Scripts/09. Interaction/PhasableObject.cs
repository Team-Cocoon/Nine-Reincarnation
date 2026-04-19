using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

public class PhasableObject : DrawOutline, IThreadInteractable, IPhasable
{
    [SerializeField] private float _targetAlpha = 0.15f;
    private SpriteRenderer _spriteRenderer;
    private CancellationTokenSource _phaseCts;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activatedColor;

    [Header("----- 상호작용 조절 ------")]
    [SerializeField] private bool _possibleActive;
    [SerializeField] private bool _isActivated;
    [SerializeField] private bool _isClickControlToSelf;
    [SerializeField] private bool _isHoverControlToSelf;
    [SerializeField] private float _interactionDistance;
    [SerializeField] private ThreadCompatibility _allowedThreads = ThreadCompatibility.Blue;
    private Vector2 _playerPosition => InputManager.Instance.CurPlayer.transform.position;
    private Collider2D _playerCollider2D => InputManager.Instance.CurPlayer.transform.GetComponent<Collider2D>();

    private Collider2D _collider2D;

    public ThreadCompatibility AllowedThreads => _allowedThreads;
    public bool IsClickControlToSelf { get => _isClickControlToSelf; }
    public override bool IsHoverControlToSelf { get => _isHoverControlToSelf; }

    public bool IsConnected => _isActivated;

    protected override void Awake()
    {
        base.Awake();
        _possibleActive = false;
        _isActivated = false;
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsOutline == true)
        {
            float dist = Vector2.Distance(_playerPosition, transform.position);

            //상호 작용 불가능한 거리면
            if (dist > _interactionDistance)
            {
                if (!_possibleActive) return;

                OutlineColor = _inactiveColor;
                _possibleActive = false;

                //이미 상호작용 중이라면
                if (_isActivated)
                {
                    PhaseOut();
                    IsOutline = false;
                }
            }
            //상호 작용 가능한 거리면
            else
            {
                if (_isActivated || _possibleActive) return;

                OutlineColor = _activeColor;
                _possibleActive = true;
            }
        }
    }

    public void PhaseIn()
    {
        OutlineColor = _activatedColor;
        _isActivated = true;
        Debug.Log("청연 연결 시작");

        // 중복 실행 방지: 이전 작업이 있다면 취소
        _phaseCts?.Cancel();
        _phaseCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        // 비동기 프로세스 시작
        ExecutePhaseSequence(_playerCollider2D, _phaseCts.Token).Forget();
    }

    private async UniTaskVoid ExecutePhaseSequence(Collider2D playerCollider, CancellationToken token)
    {
        try
        {
            InputManager.Instance.CurPlayer.AddActivePhasing();

            _isActivated = true;
            OutlineColor = _activatedColor;

            _spriteRenderer.DOFade(_targetAlpha, 0.5f).SetLink(gameObject);

            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(_collider2D, playerCollider, true);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: token);
            
            InputManager.Instance.CurPlayer.RemoveActivePhasing();
            await _spriteRenderer.DOFade(1f, 1f).SetLink(gameObject).ToUniTask(cancellationToken: token);

            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(_collider2D, playerCollider, false);
            }
            PhaseOut();
        }
        catch (OperationCanceledException)
        {
            InputManager.Instance.CurPlayer.RemoveActivePhasing();
        }
    }

    public void PhaseOut()
    {
        OutlineColor = _activeColor;
        _isActivated = false;
        Debug.Log("청연 연결 끝");

        _spriteRenderer.DOKill();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);

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
        //이미 상호작용 중이면 리턴
        if (_isActivated)
        {
            return;
        }

        //상호 작용 가능하면 색 변경
        if (_possibleActive)
        {
            OutlineColor = _activeColor;
        }
        else
        {
            OutlineColor = _inactiveColor;
        }

        base.EnableHoverInteraction();
    }

    public void DisableClickInteraction()
    {
        return;
    }

    public override void DisableHoverInteraction()
    {
        //이미 상호작용 중이라면 리턴
        if (_isActivated)
        {
            return;
        }

        base.DisableHoverInteraction();
        return;
    }
}
