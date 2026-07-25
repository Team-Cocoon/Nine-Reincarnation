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

    [Header("----- 통과(막힘) 처리 -----")]
    [SerializeField] private string _solidLayerName = "Obstacle";   //청연 미연결 시 플레이어를 막는 레이어
    private Collider2D _solidBlocker;                               //플레이어 이동을 막는 전용 콜라이더(자식)

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

        CreateSolidBlocker();
    }

    // 플레이어의 레이캐스트 모터(PlatformerRaycastMotor2D)는 특정 레이어만 '막힘'으로 판정한다.
    // 본체는 감지/호버/클릭용 Interaction 레이어를 유지해야 하므로, 막힘 전용 콜라이더를
    // solid 레이어(_solidLayerName) 위에 자식으로 하나 더 만들어 이동을 막는다.
    private void CreateSolidBlocker()
    {
        int solidLayer = LayerMask.NameToLayer(_solidLayerName);
        if (solidLayer < 0)
        {
            Debug.LogWarning($"[PhasableObject] '{_solidLayerName}' 레이어를 찾을 수 없어 막힘 블로커를 생성하지 못했습니다.");
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
            // BoxCollider2D가 아니면 본체 콜라이더 경계(로컬 기준)로 근사한다.
            box.size = _collider2D.bounds.size;
            box.offset = transform.InverseTransformPoint(_collider2D.bounds.center);
        }

        _solidBlocker = box;
        SetSolid(true);   //시작은 막힘 상태
    }

    // true면 플레이어가 막히고, false면 통과한다(페이즈 중).
    protected virtual void SetSolid(bool solid)
    {
        if (_solidBlocker != null) _solidBlocker.enabled = solid;
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

                //이미 상호작용 중이라면 (페이즈 태스크/카운트/충돌까지 온전히 정리)
                if (_isActivated)
                {
                    ForceDisconnect();
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
        ExecutePhaseSequence(_phaseCts.Token).Forget();
    }

    private async UniTaskVoid ExecutePhaseSequence(CancellationToken token)
    {
        // AddActivePhasing과 RemoveActivePhasing이 정확히 1:1로 짝이 되도록 추적한다.
        bool phasingCounted = false;
        try
        {
            InputManager.Instance.CurPlayer.AddActivePhasing();
            phasingCounted = true;

            _isActivated = true;
            OutlineColor = _activatedColor;

            _spriteRenderer.DOFade(_targetAlpha, 0.5f).SetLink(gameObject);

            SetSolid(false);   //페이즈 중에는 플레이어가 통과할 수 있게 막힘 콜라이더를 끈다
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: token);

            InputManager.Instance.CurPlayer.RemoveActivePhasing();
            phasingCounted = false;
            await _spriteRenderer.DOFade(1f, 1f).SetLink(gameObject).ToUniTask(cancellationToken: token);

            PhaseOut();   //충돌 복구 포함
        }
        catch (OperationCanceledException)
        {
            if (phasingCounted)
            {
                InputManager.Instance.CurPlayer.RemoveActivePhasing();
            }
            PhaseOut();   //충돌 복구 포함
        }
    }

    // 유지 시간이 남아 있어도 즉시 연결 전 상태로 되돌린다(청연 전환/취소 시 호출).
    public void ForceDisconnect()
    {
        if (!_isActivated) return;

        // 진행 중인 페이즈 시퀀스를 취소한다(카운트 정리는 catch에서 처리).
        _phaseCts?.Cancel();

        // 충돌/시각은 즉시 복구한다.
        PhaseOut();
    }

    public void PhaseOut()
    {
        OutlineColor = _activeColor;
        _isActivated = false;
        Debug.Log("청연 연결 끝");

        // 페이즈가 어떤 경로로 끝나든(자연 만료, 취소, 전환, 거리 이탈, 죽음) 다시 막힘 상태로 되돌린다.
        SetSolid(true);

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
