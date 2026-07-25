using System.Collections;
using Unity.Collections;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Player.Controller;

public interface IThreadThrower
{
    bool IsExpired();
}

public enum ThrowThreadState
{
    Idle,
    Throwing,
    Exist,
    Deleting
}

public enum ThreadType
{
    Red,
    Blue
}

public abstract class ThrowThread : Thread, IThreadThrower
{
    [Header("실 종류")]
    [SerializeField] private ThreadType _threadType;
    [Header("목표 지점")]
    public Transform targetTransform;
    public Vector3 targetPos;
    [Header("한계 거리")]
    [SerializeField] protected float limitDistance = 5f;
    [Header("속도")]
    [SerializeField] private float _throwSpeed = 20f;
    [Header("빗맞았을 때 연출")]
    [SerializeField] private float _fallSpeed = 10f;

    [SerializeField] private string[] _interactionLayerNames = { "Interaction" };

    protected PlayerController _player;
    protected ThrowThreadState _state;
    
    public ThrowThreadState CurrentState => _state;

    protected IClickInteractableToggle clickable;
    private int maxSegmentCount;
    private Vector3 prevNodePos;

    public event Action OnConnected;
    public event Action OnDisconnected;

    public void SetStart(Transform transform)
    {
        _startTransform = transform;
        if (_startTransform == null) return;

        _player = _startTransform.GetComponent<PlayerController>();
        if (_lineRenderer != null && segments.IsCreated)
        {
            InitThread();
            _lineRenderer.enabled = false;   //던지기 전(Idle)에는 실을 숨긴다
        }
    }

    // 던지는 실은 시작 시 보이면 안 된다. 장식용 로프를 만드는 base.Start(CreateRope)를 호출하지 않고
    // 렌더러를 꺼둔 Idle 상태로 대기한다. 실제 세그먼트는 던질 때 StartThrowing→InitThread에서 구성된다.
    protected override void Start()
    {
        Initialize();
        _state = ThrowThreadState.Idle;
        if (_lineRenderer != null) _lineRenderer.enabled = false;
    }

    protected override void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_startTransform != null)
        {
            _player = _startTransform.GetComponent<PlayerController>();
            InitThread();
        }

        // Idle 상태이므로 어떤 경우에도 시작 시에는 실을 숨긴다.
        if (_lineRenderer != null) _lineRenderer.enabled = false;
    }
    
    public void ClickEvent(Vector2 targetPosition)
    {
        switch (_state)
        {
            case ThrowThreadState.Idle:
                if (CanThrow())
                {
                    StartThrowing(targetPosition);
                }
                break;
            case ThrowThreadState.Exist:
                // 이미 연결된 상태에서 '다른' 연결 가능한 오브젝트를 클릭하면
                // 이전 실을 즉시 해제함과 동시에 새 오브젝트로 다시 연결한다(전환).
                if (TryGetConnectableTarget(targetPosition, out Transform newTarget) && newTarget != targetTransform)
                {
                    CancelConnection();
                    StartThrowing(targetPosition);
                }
                else
                {
                    // 빈 공간이나 현재 연결된 대상을 클릭하면 연결을 해제한다(수동 취소).
                    OnManualCancel();
                    StartDeleting();
                }
                break;
            case ThrowThreadState.Throwing:
            case ThrowThreadState.Deleting:
                break;
        }
    }

    // 클릭 지점에 이 실로 연결 가능한 오브젝트가 있는지 검사한다.
    private bool TryGetConnectableTarget(Vector2 position, out Transform target)
    {
        target = null;
        if (_startTransform == null) return false;

        float dist = Vector3.Distance(_startTransform.position, position);
        if (dist > limitDistance) return false;

        Collider2D hit = Physics2D.OverlapPoint(position, LayerMask.GetMask(_interactionLayerNames));
        if (hit == null) return false;

        var threadTarget = hit.GetComponent<IThreadInteractable>();
        if (threadTarget == null) return false;
        if (!threadTarget.AllowedThreads.HasFlag(ConvertToFlag(_threadType))) return false;

        target = hit.transform;
        return true;
    }

    // 페이드 없이 현재 연결을 즉시 끊는다(다른 오브젝트로 즉시 전환할 때 사용).
    // 대상 해제 방식은 실 종류마다 다르므로 ReleaseTarget()으로 분리한다.
    protected virtual void CancelConnection()
    {
        StopAllCoroutines();
        _currentJobHandle.Complete();

        ReleaseTarget();

        targetTransform = null;
        clickable = null;

        if (_endTransform != null)
        {
            _endTransform.SetParent(null);
            if (_startTransform != null) _endTransform.position = _startTransform.position;
        }

        OnDisconnected?.Invoke();

        if (_startTransform != null) InitThread();
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
            ResetAlpha();
        }

        _state = ThrowThreadState.Idle;
    }

    // 전환 시 이전 대상의 활성 상태를 해제한다.
    // 기본(홍연/Feather)은 EnableClickInteraction() 토글로 비활성화된다.
    protected virtual void ReleaseTarget()
    {
        clickable?.EnableClickInteraction();
    }

    // 사용자가 직접 연결을 취소할 때 호출되는 훅.
    // 자연 만료(코루틴/시간 경과)와 구분하기 위해 클릭 취소 경로에서만 실행된다.
    protected virtual void OnManualCancel() { }

    // ✨ 추가됨: 외부에서 기존 실을 강제로 끊을 때 호출하는 메서드
    public void ForceCancel()
    {
        if (_state == ThrowThreadState.Exist || _state == ThrowThreadState.Throwing)
        {
            StartDeleting();
        }
    }

    public virtual void ResetThread()
    {
        StopAllCoroutines();
        _currentJobHandle.Complete();
        bool wasActive = _state != ThrowThreadState.Idle;
        // 대상 해제는 실 종류마다 다르다(홍연=Feather 토글 해제, 청연=페이즈/충돌 강제 종료).
        // 청연 대상에게 EnableClickInteraction()은 재활성화(PhaseIn)이므로 여기서 직접 부르면 안 된다.
        ReleaseTarget();
        targetTransform = null;
        clickable = null;
        _state = ThrowThreadState.Idle;
        if (wasActive) OnDisconnected?.Invoke();

        if (_endTransform != null)
        {
            _endTransform.SetParent(null);
            if (_startTransform != null) _endTransform.position = _startTransform.position;
        }

        if (_startTransform != null) InitThread();
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
            ResetAlpha();
        }
    }

    protected virtual bool CanThrow() => true;

    protected override void UpdateThread()
    {
        if (!segments.IsCreated) return;

        if (_state == ThrowThreadState.Exist)
        {
            if (targetTransform == null)
            {
                OnDisconnected?.Invoke();
                StartDeleting();
                return;
            }

            if (IsExpired())
            {
                OnDisconnected?.Invoke();
                StartDeleting();
                return;
            }
            UpdateThreadVisualization();
        }
        else if (_state == ThrowThreadState.Deleting)
        {
            if (targetTransform == null && _endTransform != null)
            {
                _endTransform.position += _gravity * Time.deltaTime * 0.5f;
            }

            UpdateThreadVisualization();
        }
    }

    protected virtual void UpdateThreadVisualization()
    {
        UpdateSegments();
    }

    protected void InitThread()
    {
        if (_startTransform == null) return;
        if (_endTransform == null)
        {
            GameObject autoGeneratedEnd = new GameObject($"[{gameObject.name}]_EndTransform_Recovered");
            _endTransform = autoGeneratedEnd.transform;
        }
        _endTransform.SetParent(null); 

        if (segments.IsCreated)
        {
            _currentJobHandle.Complete();
            segments.Dispose();
        }
        segmentCount = 2;
        _endTransform.position = _startTransform.position;
        prevNodePos = _startTransform.position;
        Vector3 dir = (targetPos - _startTransform.position).normalized;
        float dist = Vector3.Distance(_startTransform.position, targetPos);
        if (dist > limitDistance)
        {
            targetPos = _startTransform.position + dir * limitDistance;
        }
        maxSegmentCount = (int)((_startTransform.position - targetPos).magnitude * 4);

        maxSegmentCount = Mathf.Max(maxSegmentCount, 2);
        _segmentPositions = new Vector3[maxSegmentCount];
        segments = new NativeArray<Segment>(maxSegmentCount, Allocator.Persistent);
        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 pos = _startTransform.position;
            segments[i] = new Segment(pos);
            _segmentPositions[i] = pos;
        }
        _lineRenderer.positionCount = segmentCount;
        _lineRenderer.SetPositions(_segmentPositions);
    }

    void NormalizeSegments()
    {
        if (!segments.IsCreated) return;

        for (int i = 0; i < maxSegmentCount; i++)
        {
            float t = (float)i / (maxSegmentCount - 1);
            Vector3 pos = Vector3.Lerp(_startTransform.position, targetPos, t);
            segments[i] = new Segment(pos);
        }
    }

    void AddSegments()
    {
        if (_endTransform == null || !segments.IsCreated) return;

        float dist = Vector3.Distance(prevNodePos, _endTransform.position);

        if (dist >= segmentDist)
        {
            segments[segmentCount] = new Segment(_endTransform.position);
            segmentCount++;

            prevNodePos = _endTransform.position;
        }
    }

    private void NormalizeActiveSegments()
    {
        if (_startTransform == null || _endTransform == null || !segments.IsCreated || segmentCount < 2) return;

        Vector3 start = _startTransform.position;
        Vector3 end = _endTransform.position;
        int lastIndex = segmentCount - 1;
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)lastIndex;
            segments[i] = new Segment(Vector3.Lerp(start, end, t));
        }
    }

    protected void StartThrowing(Vector2 mousePos)
    {
        _lineRenderer.enabled = true;
        _state = ThrowThreadState.Throwing;
        
        Collider2D hit = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask(_interactionLayerNames));
        float dist = Vector3.Distance(_startTransform.position, mousePos);

        if (hit != null && dist <= limitDistance)
        {
            targetTransform = hit.transform;
            targetPos = targetTransform.position;
        }
        else
        {
            targetTransform = null;

            if (dist <= limitDistance)
            {
                targetPos = mousePos;
            }
            else
            {
                Vector3 dir = ((Vector3)mousePos - _startTransform.position).normalized;
                targetPos = _startTransform.position + dir * limitDistance;
            }
        }

        InitThread();

        if (targetTransform != null) 
        {
            var threadTarget = targetTransform.GetComponent<IThreadInteractable>();
            bool isCompatible = false;

            if (threadTarget != null)
                isCompatible = threadTarget.AllowedThreads.HasFlag(ConvertToFlag(this._threadType));

            if (isCompatible)
            {
                clickable = targetTransform.GetComponent<IClickInteractableToggle>();
                threadTarget.OnThreadHit(this._threadType);

                if (_endTransform != null)
                {
                    _endTransform.SetParent(targetTransform);
                    _endTransform.localPosition = Vector2.zero;
                }
                targetPos = targetTransform.position;

                StartCoroutine(Throwing(() =>
                {
                    _state = ThrowThreadState.Exist;
                    AudioManager.Instance?.PlaySfx(AudioManager.Sfx.LinkThread);
                    OnConnected?.Invoke();
                    ThrowingEvent();
                }));
            }
            else
            {
                StartCoroutine(Throwing(StartDeleting));
            }
        }
        else
        {
            StartCoroutine(Throwing(StartDeleting));
        }
    }

    private ThreadCompatibility ConvertToFlag(ThreadType state)
    {
        return state == ThreadType.Red ? ThreadCompatibility.Red : ThreadCompatibility.Blue;
    }

    protected virtual void ThrowingEvent() { }
    
    protected virtual void StartDeleting()
    {
        _state = ThrowThreadState.Deleting;
        if (_endTransform != null)
        {
            _endTransform.SetParent(null); 
        }
        OnDisconnected?.Invoke();
    }

    private IEnumerator Throwing(System.Action onComplete)
    {
        while (true)
        {
            // ✨ 수정됨: 날아가는 도중에 다른 실을 던져서 강제로 Cancel(취소)된 경우, 더 이상 날아가지 않도록 코루틴 즉시 종료
            if (_state != ThrowThreadState.Throwing) yield break;

            if (_endTransform == null || !segments.IsCreated) yield break;

            float distToTarget = Vector3.Distance(_endTransform.position, targetPos);
            Vector3 dir = (targetPos - _endTransform.position).normalized;

            float step = Mathf.Min(_throwSpeed * Time.deltaTime, distToTarget);
            _endTransform.position += dir * step;

            if (segmentCount >= maxSegmentCount || distToTarget <= 0.01f)
            {
                if (_endTransform == null || !segments.IsCreated) yield break;

                _endTransform.position = targetPos;
                segments[maxSegmentCount - 1] = new Segment(_endTransform.position);
                segmentCount = maxSegmentCount;
                NormalizeSegments();
                onComplete?.Invoke();
                yield break;
            }
            AddSegments();
            // Keep every visible throwing node on the same current player anchor.
            // Otherwise old launch-position nodes remain until NormalizeSegments()
            // and appear to snap to the player exactly when the throw completes.
            NormalizeActiveSegments();

            yield return null;
        }
    }
    
    protected virtual void ResetAlpha()
    {
        if (_lineRenderer != null)
        {
            Color c = _lineRenderer.startColor;
            c.a = 1f;
            _lineRenderer.startColor = c;
            _lineRenderer.endColor = c;
        }
    }

    protected virtual void OnDisable()
    {
        if (_endTransform != null)
        {
            _endTransform.SetParent(null);
        }
    }

    public abstract bool IsExpired();
}
