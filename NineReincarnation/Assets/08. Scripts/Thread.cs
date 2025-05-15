using System.Collections.Generic;
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

    protected LineRenderer _lineRenderer;
    protected LayerMask _hitLayer;
    protected List<Segment> segments = new List<Segment>();

    private Vector2 _gravity = new Vector2(0, -9.81f);
    protected abstract void UpdateThread();

    protected virtual void Awake()
    {
        Initialize();
        CreateRope();
    }
    protected virtual void FixedUpdate()
    {
        UpdateThread();
    }
    protected virtual void Initialize() 
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    /// <summary>
    /// 각 노드정보 갱신
    /// </summary>
    protected void UpdateSegments()
    {
        Vector2 gravityEffect = _gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].velocity = segments[i].position - segments[i].prevPosition;
            segments[i].prevPosition = segments[i].position;
            segments[i].position += gravityEffect;
            segments[i].position += segments[i].velocity;
        }
    }
    /// <summary>
    /// 각 노드의 운동방향 갱신
    /// </summary>
    protected void ApplyConstraint()
    {
        segments[0].position = _startTransform.position;
        segments[segments.Count - 1].position = _endTransform.position;
        for (int i = 0; i < segments.Count - 1; i++)
        {
            float distance = (segments[i].position - segments[i + 1].position).magnitude;
            float difference = segmentDist - distance;
            Vector2 dir = (segments[i + 1].position - segments[i].position).normalized;

            Vector2 movement = dir * difference;
            if (i == 0)
                segments[i + 1].position += movement;
            else if (i == segments.Count - 2)
                segments[i].position -= movement;
            else
            {
                segments[i].position -= movement * 0.5f;
                segments[i + 1].position += movement * 0.5f;
            }
        }
    }
    /// <summary>
    /// 실 그리기
    /// </summary>
    protected void RenderThread()
    {
        _lineRenderer.startWidth = _lineRenderer.endWidth = threadWidth;
        Vector3[] segmentPositions = new Vector3[segments.Count];
        for (int i = 0; i < segments.Count; i++)
        {
            segmentPositions[i] = segments[i].position;
        }
        _lineRenderer.positionCount = segmentPositions.Length;
        _lineRenderer.SetPositions(segmentPositions);
    }
    /// <summary>
    /// 실의 충돌 시 위치 보정
    /// </summary>
    protected void AdjustCollision()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            Vector2 dir = segments[i].position - segments[i].prevPosition;

            RaycastHit2D hit = Physics2D.CircleCast(segments[i].position, threadWidth * 0.5f, dir.normalized, 0f, _hitLayer);

            if (hit) 
            {
                segments[i].position = hit.point + hit.normal * threadWidth * 0.5f;
                segments[i].prevPosition = segments[i].position;
            }
        }
    }
    private void CreateRope()
    {
        Vector2 segmentPos = _endTransform.position;
        for (int i = 0; i < segmentCount; i++)
        {
            segments.Add(new Segment(segmentPos));
            segmentPos.y -= segmentDist;
        }
    }
}
public class Segment
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