using System.Collections;
using Unity.Collections;
using UnityEngine;

public enum ThrowThreadState
{
    Idle,
    Throwing,
    Exist,
    Deleting
}

public class ThrowThread : Thread
{
    [Header("목표 지점")]
    public Transform targetTransform;
    public Vector3 targetPos;
    [Header("한계 거리")]
    [SerializeField] private float limitDistance = 5f;
    [Header("속도")]
    [SerializeField] private float _throwSpeed = 1f;

    private ThrowThreadState _state;

    private int maxSegmentCount;
    private Vector3 prevNodePos;

    protected override void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        InitThread();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickEvent();
        }
    }
    /*  이것만 호출하면 됨 */
    public void ClickEvent()
    {
        switch (_state)
        {
            case ThrowThreadState.Idle:
                StartThrowing();
                break;
            case ThrowThreadState.Exist:
                StartDeleting();
                break;
            case ThrowThreadState.Throwing:
            case ThrowThreadState.Deleting:
                break;
        }
    }
    protected override void UpdateThread()
    {
        if (_state == ThrowThreadState.Exist)
        {
            if (targetTransform != null)
            {
                targetPos = targetTransform.position;
                float currentDistance = Vector3.Distance(_startTransform.position, targetPos);

                if (currentDistance > limitDistance)
                {
                    StartDeleting();
                    return;
                }
            }
            UpdateSegments();
        }
    }
    void InitThread()
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

    private void StartThrowing()
    {
        _lineRenderer.enabled = true;
        _state = ThrowThreadState.Throwing;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 20, LayerMask.GetMask("Interaction"));

        targetTransform = null;

        if (hit.collider != null)
        {
            targetTransform = hit.collider.transform;
            targetPos = targetTransform.position;
        }
        else
        {
            targetPos = mousePos;
        }

        InitThread();

        hit = Physics2D.Raycast(targetPos, Vector2.zero, 20, LayerMask.GetMask("Interaction"));
        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            clickable?.OnClicked();
            targetTransform = hit.collider.transform;

            _endTransform.SetParent(targetTransform);
            _endTransform.localPosition = Vector2.zero;

            targetPos = targetTransform.position;

            StartCoroutine(Throwing(() => { _state = ThrowThreadState.Exist; }));
        }
        else
        {
            StartCoroutine(Throwing(StartDeleting));
        }
    }
    private void StartDeleting()
    {
        _state = ThrowThreadState.Deleting;
        StartCoroutine(Disappearing(() => { _state = ThrowThreadState.Idle; }));
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

    private IEnumerator Disappearing(System.Action onComplete)
    {
        float elapsedTime = 0f;
        Color startColor = _lineRenderer.startColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / 1f);

            _lineRenderer.startColor = currentColor;
            _lineRenderer.endColor = currentColor;

            yield return null;
        }
        _lineRenderer.startColor = endColor;
        _lineRenderer.endColor = endColor;

        InitThread();
        _lineRenderer.enabled = false;
        ResetAlpha();
        onComplete?.Invoke();
    }
    private void ResetAlpha()
    {
        Color resetColor = _lineRenderer.startColor;
        resetColor.a = 1f;
        _lineRenderer.startColor = resetColor;
        _lineRenderer.endColor = resetColor;
    }

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
