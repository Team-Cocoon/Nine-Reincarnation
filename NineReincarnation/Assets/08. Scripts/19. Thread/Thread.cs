using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/* 실 최상위 부모 */
public abstract class Thread : MonoBehaviour
{
    [Header("ThreadData")]
    [Tooltip("실의 노드 개수")]
    public int segmentCount = 3;
    [SerializeField] protected float segmentDist = 0.4f;
    [SerializeField] protected float threadWidth = 0.1f;
    [SerializeField] protected Transform _startTransform;
    [SerializeField] protected Transform _endTransform;
    [SerializeField] private float _segAmount = 0.3f; // 중심 축 쳐지는 깊이 조절

    protected LineRenderer _lineRenderer;
    protected EdgeCollider2D _edgeCollider;
    protected Vector3[] _segmentPositions;

    protected NativeArray<Segment> segments;
    protected JobHandle _currentJobHandle;

    protected Vector3 _gravity = new Vector3(0, -5f, 0);

    protected abstract void UpdateThread();

    protected virtual void Start()
    {
        Initialize();
        if (HasValidAnchors())
        {
            CreateRope();
        }
        else if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
        }
    }
    protected void OnDestroy()
    {
        if (segments.IsCreated)
        {
            _currentJobHandle.Complete();
            segments.Dispose();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (!HasValidAnchors() || !segments.IsCreated) return;
        if (_currentJobHandle.IsCompleted)
        {
            UpdateThread();
        }
    }
    protected virtual void LateUpdate()
    {
        if (!HasValidAnchors() || _lineRenderer == null || !segments.IsCreated)
        {
            if (_lineRenderer != null) _lineRenderer.enabled = false;
            return;
        }
        _currentJobHandle.Complete();
        RenderThread();
    }
    protected virtual void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        segmentCount = Mathf.Max(segmentCount, 2);
        _segmentPositions = new Vector3[segmentCount];
        segments = new NativeArray<Segment>(segmentCount, Allocator.Persistent);
    }

    /// <summary>
    /// 실 그리기
    /// </summary>
    protected virtual void RenderThread()
    {
        if (!HasValidAnchors() || _lineRenderer == null || !segments.IsCreated) return;
        _currentJobHandle.Complete();

        int activeCount = GetActiveSegmentCount();
        if (activeCount <= 0)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        segmentCount = activeCount;

        Segment start = segments[0];
        start.position = start.prevPosition = _startTransform.position;
        segments[0] = start;

        if (_endTransform != null && activeCount > 1)
        {
            int endIndex = activeCount - 1;
            Segment end = segments[endIndex];
            end.position = end.prevPosition = _endTransform.position;
            segments[endIndex] = end;
        }

        _lineRenderer.startWidth = _lineRenderer.endWidth = threadWidth;
        _lineRenderer.positionCount = activeCount;
        for (int i = 0; i < activeCount; i++)
        {
            _segmentPositions[i] = segments[i].position;
            _lineRenderer.SetPosition(i, _segmentPositions[i]);
        }
    }

    protected void UpdateSegments()
    {
        if (!HasValidAnchors() || !segments.IsCreated) return;

        int activeCount = GetActiveSegmentCount();
        if (activeCount < 2) return;

        segmentCount = activeCount;
        UpdateSegmentJob updateJob = new UpdateSegmentJob
        {
            gravity = _gravity,
            deltaTime = Time.deltaTime,
            segments = segments
        };
        ApplyConstraintJob applyJob = new ApplyConstraintJob
        {
            segments = segments,
            segmentCount = activeCount,
            segmentDist = segmentDist,
            startPosition = _startTransform.position,
            endPosition = _endTransform.position
        };

        JobHandle UpdateSegmentHandle = updateJob.Schedule(activeCount, 64, _currentJobHandle);
        JobHandle ApplyConstraintHandle = applyJob.Schedule(UpdateSegmentHandle);
        _currentJobHandle = ApplyConstraintHandle;
    }

    protected int GetActiveSegmentCount()
    {
        if (!segments.IsCreated || _segmentPositions == null) return 0;

        int capacity = Mathf.Min(segments.Length, _segmentPositions.Length);
        return Mathf.Clamp(segmentCount, 0, capacity);
    }

    private void CreateRope()
    {
        if (!HasValidAnchors() || !segments.IsCreated) return;
        Vector2 start = _startTransform.position;
        Vector2 end = _endTransform.position;
        int segmentCount = GetActiveSegmentCount();
        if (segmentCount < 2) return;
        this.segmentCount = segmentCount;

        float totalLength = segmentDist * (segmentCount - 1);
        Vector2 dir = (end - start).normalized;
        Vector2 right = new Vector2(1, 0); // x축 기준 방향

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1); // 0 ~ 1
            float x = segmentDist * i;

            // 축 쳐지는 형태: 사인 기반으로 y값 조절
            float y = -Mathf.Sin(Mathf.PI * t) * _segAmount;

            // 방향 벡터를 따라 x축으로 이동 + y는 아래로 sag
            Vector2 pos = start + dir * x + Vector2.up * y;

            //segments.Add(new Segment(pos));
            segments[i] = new Segment(pos);
        }
    }

    protected bool HasValidAnchors()
    {
        return _startTransform != null && _endTransform != null;
    }
}

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
struct UpdateSegmentJob : IJobParallelFor
{
    [ReadOnly] public Vector2 gravity;
    [ReadOnly] public float deltaTime;
    public NativeArray<Segment> segments;
    public void Execute(int index)
    {
        Segment seg = segments[index];
        seg.velocity = seg.position - seg.prevPosition;
        seg.prevPosition = seg.position;
        seg.position += gravity * deltaTime * deltaTime;
        seg.position += seg.velocity;
        segments[index] = seg;
    }
}

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
public struct ApplyConstraintJob : IJob
{
    public NativeArray<Segment> segments;
    public int segmentCount;
    public float segmentDist;
    public Vector2 startPosition;
    public Vector2 endPosition;

    public void Execute()
    {
        int activeCount = segmentCount;
        if (activeCount > segments.Length)
        {
            activeCount = segments.Length;
        }
        if (activeCount <= 0) return;

        Segment startSeg = segments[0];
        startSeg.position = startPosition;
        segments[0] = startSeg;

        if (activeCount == 1) return;

        Segment endSeg = segments[activeCount - 1];
        endSeg.position = endPosition;
        segments[activeCount - 1] = endSeg;

        for (int i = 0; i < activeCount - 1; i++)
        {
            Segment segA = segments[i];
            Segment segB = segments[i + 1];

            float distance = (segA.position - segB.position).magnitude;
            float difference = segmentDist - distance;
            Vector2 dir = (segB.position - segA.position).normalized;
            Vector2 movement = dir * difference;

            if (i == 0)
            {
                segB.position += movement;
            }
            else if (i == activeCount - 2)
            {
                segA.position -= movement;
            }
            else
            {
                segA.position -= movement * 0.5f;
                segB.position += movement * 0.5f;
            }

            segments[i] = segA;
            segments[i + 1] = segB;
        }
    }
}

public struct Segment
{
    public Vector2 prevPosition;
    public Vector2 position;
    public Vector2 velocity;

    public Segment(Vector2 _position)
    {
        prevPosition = _position;
        position = _position;
        velocity = Vector2.zero;
    }
}
