using System.Collections;
using Unity.Collections;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Player.Controller;

public interface IThreadThrower
{
    /// <summary>
    /// 실이 Exist상태일 때, 사라질지 말지 확인하는 함수
    /// </summary>
    /// <returns></returns>
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

    protected PlayerController _player;
    protected ThrowThreadState _state;
    protected IClickInteractableToggle clickable;
    private int maxSegmentCount;
    private Vector3 prevNodePos;

    /* UI관련 */
    public event Action OnConnected;
    public event Action OnDisconnected;

    public void SetStart(Transform transform)
    {
        _startTransform = transform;
    }

    protected override void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _player = _startTransform.GetComponent<PlayerController>();
        InitThread();
    }
    /*  이것만 호출하면 됨 */
    public void ClickEvent()
    {
        switch (_state)
        {
            case ThrowThreadState.Idle:
                if (CanThrow())
                {
                    StartThrowing();
                }
                break;
            case ThrowThreadState.Exist:
                StartDeleting();
                break;
            case ThrowThreadState.Throwing:
            case ThrowThreadState.Deleting:
                break;
        }
    }

    protected virtual bool CanThrow() => true;

    protected override void UpdateThread()
    {
        if (_state == ThrowThreadState.Exist)
        {
            if (targetTransform != null)
            {
                if (IsExpired())
                {
                    OnDisconnected?.Invoke();
                    StartDeleting();
                    return;
                }
            }
            UpdateThreadVisualization();
        }
        else if (_state == ThrowThreadState.Deleting)
        {
            if (targetTransform == null)
            {
                _endTransform.position += _gravity * Time.deltaTime * 0.5f;

                // NormalizeSegments();
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
        if (segments.IsCreated)
        {
            _currentJobHandle.Complete();
            segments.Dispose();
        }
        segmentCount = 2;
        _endTransform.position = _startTransform.position;
        Vector3 dir = (targetPos - _startTransform.position).normalized;
        float dist = Vector3.Distance(_startTransform.position, targetPos);
        if (dist > limitDistance)
        {
            targetPos = _startTransform.position + dir * limitDistance;
        }
        // targetPos 보정
        maxSegmentCount = (int)((_startTransform.position - targetPos).magnitude * 4);

        maxSegmentCount = Mathf.Max(maxSegmentCount, 2);
        _segmentPositions = new Vector3[maxSegmentCount];
        segments = new NativeArray<Segment>(maxSegmentCount, Allocator.Persistent);
        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 pos = _startTransform.position;
            segments[i] = new Segment(pos);
        }
        _lineRenderer.positionCount = segmentCount;
        _lineRenderer.SetPositions(_segmentPositions);
    }

    void NormalizeSegments()
    {
        for (int i = 0; i < maxSegmentCount; i++)
        {
            float t = (float)i / (maxSegmentCount - 1);
            Vector3 pos = Vector3.Lerp(_startTransform.position, targetPos, t);
            segments[i] = new Segment(pos);
        }
    }

    void AddSegments()
    {
        float dist = Vector3.Distance(prevNodePos, _endTransform.position);

        if (dist >= segmentDist)
        {
            segments[segmentCount] = new Segment(_endTransform.position);
            segmentCount++;

            prevNodePos = _endTransform.position;
        }
    }

    protected void StartThrowing()
    {
        _lineRenderer.enabled = true;
        _state = ThrowThreadState.Throwing;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("Interaction"));

        float dist = Vector3.Distance(_startTransform.position, mousePos);

        // 2. 타겟 설정 (거리가 한계치 이내일 때만 hit 인정)
        if (hit != null && dist <= limitDistance)
        {
            targetTransform = hit.transform;
            targetPos = targetTransform.position;
        }
        else
        {
            // 거리가 너무 멀면 hit이 있어도 허공으로 취급
            targetTransform = null;

            if (dist <= limitDistance)
            {
                // 1. 사거리 안쪽을 클릭했다면? 그 지점까지만 던짐
                targetPos = mousePos;
            }
            else
            {
                // 2. 사거리 밖을 클릭했다면? 방향은 유지하되 거리는 제한
                Vector3 dir = ((Vector3)mousePos - _startTransform.position).normalized;
                targetPos = _startTransform.position + dir * limitDistance;
            }
        }

        InitThread();

        // 3. 연결 가능 여부 최종 판별
        if (targetTransform != null) // 위에서 거리 체크를 통과한 경우에만 들어옴
        {
            var threadTarget = targetTransform.GetComponent<IThreadInteractable>();
            bool isCompatible = false;

            if (threadTarget != null)
                isCompatible = threadTarget.AllowedThreads.HasFlag(ConvertToFlag(this._threadType));

            if (isCompatible)
            {
                clickable = targetTransform.GetComponent<IClickInteractableToggle>();
                threadTarget.OnThreadHit(this._threadType);

                _endTransform.SetParent(targetTransform);
                _endTransform.localPosition = Vector2.zero;
                targetPos = targetTransform.position;

                StartCoroutine(Throwing(() =>
                {
                    _state = ThrowThreadState.Exist;
                    AudioManager.Instance?.PlaySfx(AudioManager.Sfx.LinkThread);
                    OnConnected?.Invoke();
                    /* 이 부분 상속처리함 */
                    ThrowingEvent();
                }));
            }
            else
            {
                // 호환되지 않는 실인 경우
                StartCoroutine(Throwing(StartDeleting));
            }
        }
        else
        {
            // 거리가 멀거나 맞은 게 없는 경우 (이때 Deleting 상태에서 아래로 떨어짐)
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
        OnDisconnected?.Invoke();
    }

    private IEnumerator Throwing(System.Action onComplete)
    {
        while (true)
        {
            float distToTarget = Vector3.Distance(_endTransform.position, targetPos);

            Vector3 dir = (targetPos - _endTransform.position).normalized;

            float step = Mathf.Min(_throwSpeed * Time.deltaTime, distToTarget);
            _endTransform.position += dir * step;

            if (segmentCount >= maxSegmentCount || distToTarget <= 0.01f)
            {
                _endTransform.position = targetPos;
                segments[maxSegmentCount - 1] = new Segment(_endTransform.position);
                segmentCount = maxSegmentCount;
                NormalizeSegments();
                onComplete?.Invoke();
                yield break;
            }
            AddSegments();

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

    public abstract bool IsExpired();

    #region 포물선 운동
    //private Vector3 startPos;
    //private Vector3 targetPos;
    //private Vector3 dirXZ;
    //private float flightTime;
    //private float vY;
    //private float elapsed;

    //void Throw()
    //{
    //    if (_isThrow) return;

    //    // 최초 1회만 초기화
    //    if (!hasNextTarget && segmentCount < maxSegmentCount)
    //    {
    //        hasNextTarget = true;
    //        prevNodePos = _endTransform.position;

    //        startPos = _endTransform.position;
    //        targetPos = targetTransform.position;

    //        Vector3 diff = targetPos - startPos;
    //        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);

    //        float distXZ = diffXZ.magnitude;
    //        float distY = diff.y;

    //        dirXZ = diffXZ.normalized;

    //        // 도달 시간 (수평 거리 / 수평 속도)
    //        flightTime = distXZ / _throwSpeed;

    //        // 초기 Y속도
    //        vY = (distY - 0.5f * _gravity.y * flightTime * flightTime) / flightTime;

    //        elapsed = 0f;
    //    }

    //    float dt = Time.deltaTime;
    //    elapsed += dt;
    //    if (elapsed > flightTime) elapsed = flightTime;

    //    // 수학 공식 그대로 적용
    //    float xz = _throwSpeed * elapsed;
    //    float y = vY * elapsed + 0.5f * _gravity.y * elapsed * elapsed;

    //    _endTransform.position = startPos + dirXZ * xz + Vector3.up * y;

    //    // 목표 도착
    //    if (elapsed >= flightTime)
    //    {
    //        _endTransform.position = targetPos;
    //        segments[maxSegmentCount - 1] = new Segment(_endTransform.position);
    //        segmentCount = maxSegmentCount;
    //        NormalizeSegments();
    //        _isThrow = true;
    //    }
    //}
    //void NormalizeSegments()
    //{
    //    for (int i = 0; i < maxSegmentCount; i++)
    //    {
    //        float ratio = (float)i / (maxSegmentCount - 1);
    //        float time = flightTime * ratio;  // 전체 비행시간을 ratio로 나눔

    //        // 수평(XZ)
    //        float xz = _throwSpeed * time;
    //        Vector3 posXZ = startPos + dirXZ * xz;

    //        // 수직(Y)
    //        float y = vY * time + 0.5f * _gravity.y * time * time;
    //        posXZ.y = startPos.y + y;

    //        segments[i] = new Segment(posXZ);
    //    }
    //}
    #endregion
}
