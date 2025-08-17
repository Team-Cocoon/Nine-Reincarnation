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
    [SerializeField] private float _segAmount = 0.3f; // 중심 축 쳐지는 깊이 조절

    protected LineRenderer _lineRenderer;
    protected EdgeCollider2D _edgeCollider;
    protected List<Segment> segments = new List<Segment>();

    private Vector2 _gravity = new Vector2(0, -9.81f);
    protected abstract void UpdateThread();

    protected virtual void Start()
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
    protected virtual void RenderThread()
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
    private void CreateRope()
    {
        //Vector2 gravityEffect = _gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        //Vector2 segmentPos = _endTransform.position;
        //for (int i = 0; i < segmentCount; i++)
        //{
        //    segments.Add(new Segment(segmentPos));
        //    segmentPos.y -= segmentDist;
        //    segments[i].prevPosition = segments[i].position;
        //    segments[i].position += gravityEffect;
        //    segments[i].position += segments[i].velocity;
        //}
        Vector2 start = _startTransform.position;
        Vector2 end = _endTransform.position;
        int segmentCount = this.segmentCount;

        float totalLength = segmentDist * (segmentCount - 1);
        Vector2 dir = (end - start).normalized;
        Vector2 right = new Vector2(1, 0); // x축 기준 방향

        // x축 기준으로 정렬된 줄 만들기
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1); // 0 ~ 1
            float x = segmentDist * i;

            // 축 쳐지는 형태: 사인 기반으로 y값 조절
            float y = -Mathf.Sin(Mathf.PI * t) * _segAmount;

            // 방향 벡터를 따라 x축으로 이동 + y는 아래로 sag
            Vector2 pos = start + dir * x + Vector2.up * y;

            segments.Add(new Segment(pos));
            segments[i].prevPosition = pos;
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