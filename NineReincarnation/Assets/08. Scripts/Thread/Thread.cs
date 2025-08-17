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
    private JobHandle _currentJobHandle;

    private Vector2 _gravity = new Vector2(0, -5f);
    protected bool _isInCamera = false; // 카메라 안에 들어오는지

    protected abstract void UpdateThread();

    protected virtual void Start()
    {
        Initialize();
        CreateRope();
    }
    protected void OnDestroy()
    {
        if (segments.IsCreated)
            segments.Dispose();
    }
    protected virtual void FixedUpdate()
    {
        if (_currentJobHandle.IsCompleted)
        {
            UpdateThread();
        }
    }
    protected virtual void LateUpdate()
    {
        _currentJobHandle.Complete();
        if (_isInCamera) RenderThread();
    }
    protected virtual void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _segmentPositions = new Vector3[segmentCount];
        segments = new NativeArray<Segment>(segmentCount, Allocator.Persistent);
    }

    /// <summary>
    /// 실 그리기
    /// </summary>
    protected virtual void RenderThread()
    {
        _lineRenderer.startWidth = _lineRenderer.endWidth = threadWidth;
        for (int i = 0; i < segmentCount; i++)
        {
            _segmentPositions[i] = segments[i].position;
        }
        _lineRenderer.positionCount = _segmentPositions.Length;
        _lineRenderer.SetPositions(_segmentPositions);
    }
    protected void UpdateSegments()
    {
        UpdateSegmentJob updateJob = new UpdateSegmentJob
        {
            gravity = _gravity,
            deltaTime = Time.deltaTime,
            segments = segments
        };
        ApplyConstraintJob applyJob = new ApplyConstraintJob
        {
            segments = segments,
            segmentCount = segmentCount,
            segmentDist = segmentDist,
            startPosition = _startTransform.position,
            endPosition = _endTransform.position
        };

        JobHandle UpdateSegmentHandle = updateJob.Schedule(segmentCount, 64, _currentJobHandle);
        JobHandle ApplyConstraintHandle = applyJob.Schedule(UpdateSegmentHandle);
        _currentJobHandle = ApplyConstraintHandle;
    }

    private void CreateRope()
    {
        Vector2 start = _startTransform.position;
        Vector2 end = _endTransform.position;
        int segmentCount = this.segmentCount;

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
    protected void OnBecameVisible()
    {
        _isInCamera = true;
    }

    protected void OnBecameInvisible()
    {
        _isInCamera = false;
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
        Segment startSeg = segments[0];
        startSeg.position = startPosition;
        segments[0] = startSeg;

        Segment endSeg = segments[segmentCount - 1];
        endSeg.position = endPosition;
        segments[segmentCount - 1] = endSeg;

        for (int i = 0; i < segmentCount - 1; i++)
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
            else if (i == segmentCount - 2)
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