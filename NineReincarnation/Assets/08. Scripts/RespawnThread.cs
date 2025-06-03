using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    /// <summary>
    /// 충돌 시 한 번 일어날 이벤트
    /// </summary>
    /// <param name="go"></param>
    public void Enter(GameObject go = null);

    /// <summary>
    /// 충돌 해제 시 한 번 일어날 이벤트
    /// </summary>
    /// <param name="go"></param>
    public void Exit(GameObject go = null);
}
/* 닿으면 사라지는 실 (N초 후에 다시 생성) */
public class RespawnThread : Thread, ICollidable
{
    [Header("HitLayer")]
    [SerializeField] private LayerMask _hitLayer;

    [Header("RespawnData")]
    [SerializeField] private bool _isHit;
    [SerializeField] private float _disappearTime = 0f;
    [SerializeField] private float _respawnTime = 0f;

    private List<GameObject> _playersOnRope = new List<GameObject>();
    private Coroutine _disappearCoroutine;
    private Coroutine _appearCoroutine;

    [Header("Player Data")]
    [SerializeField] private float _damping = 10f;
    private float _initGravity;
    private float _initDamping;

    protected override void UpdateThread()
    {
        UpdateSegments();
        for (int i = 0; i < segmentCount / 2; i++)
        {
            ApplyConstraint();
            AdjustCollision();
        }
        RenderThread();
        
        //foreach (var player in _playersOnRope)
        //    StickPlayerToThread(player);
    }
    protected override void Initialize()
    {
        base.Initialize();
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }
    protected override void RenderThread()
    {
        _lineRenderer.startWidth = _lineRenderer.endWidth = threadWidth;
        Vector3[] segmentPositions = new Vector3[segments.Count];
        Vector2[] colliderPositions = new Vector2[segments.Count];
        for (int i = 0; i < segments.Count; i++)
        {
            segmentPositions[i] = segments[i].position;
            colliderPositions[i] = transform.InverseTransformPoint(segments[i].position);
        }
        _lineRenderer.positionCount = segmentPositions.Length;
        _lineRenderer.SetPositions(segmentPositions);
        _edgeCollider.edgeRadius = threadWidth * 0.5f;
        _edgeCollider.points = colliderPositions;
    }
    /// <summary>
    /// 실의 충돌 시 위치 보정
    /// </summary>
    private void AdjustCollision()
    {
        //bool isColliding = false;
        for (int i = 0; i < segments.Count; i++)
        {
            Vector2 dir = segments[i].position - segments[i].prevPosition;

            RaycastHit2D hit = Physics2D.CircleCast(segments[i].position, threadWidth * 0.5f, dir.normalized, 0f, _hitLayer);

            if (hit) // 충돌 시 위치 보정
            {
                segments[i].position = hit.point + hit.normal * threadWidth * 0.5f;
                segments[i].prevPosition = segments[i].position;
            }
           
        }
    }
    private void StickPlayerToThread(GameObject player)
    {
        if (player == null)
            return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        rb.gravityScale = 0f;
        rb.linearDamping = _damping;
    }
    private void AppearThread()
    {
        _disappearCoroutine = null;
        ResetAlpha();
        _lineRenderer.enabled = true;
        _edgeCollider.enabled = true;
    }

    private void ResetAlpha()
    {
        Color resetColor = _lineRenderer.startColor;
        resetColor.a = 1f;
        _lineRenderer.startColor = resetColor;
        _lineRenderer.endColor = resetColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log(collision.gameObject.name);
        //    _isHit = true;
        //    var player = collision.gameObject;
        //    if (!_playersOnRope.Contains(player))
        //    {
        //        _playersOnRope.Add(player);
        //    }
        //    if (_disappearCoroutine == null)
        //    {
        //        _disappearCoroutine = StartCoroutine(DisappearThreadCoroutine());
        //    }
        //}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    _isHit = false;
        //    var player = collision.gameObject;
        //    if (_playersOnRope.Contains(player))
        //    {
        //        _playersOnRope.Remove(player);
        //    }
        //}
    }
    private IEnumerator DisappearThreadCoroutine()
    {
        float waitTime = _disappearTime - 1f;
        float timer = 0f;

        while (timer < waitTime)
        {
            if (!_isHit)
            {
                AppearThread();
                yield break;  // 중간에 빠졌으면 중단
            }
            timer += Time.deltaTime;
            yield return null;
        }

        float elapsedTime = 0f;
        Color startColor = _lineRenderer.startColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < 1f)
        {
            if (!_isHit)
            {
                AppearThread();
                yield break;  // 중간에 빠졌으면 중단
            }
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / 1f);

            _lineRenderer.startColor = currentColor;
            _lineRenderer.endColor = currentColor;

            yield return null;
        }
        _lineRenderer.startColor = endColor;
        _lineRenderer.endColor = endColor;

        _lineRenderer.enabled = false;
        _edgeCollider.enabled = false;

        yield return _appearCoroutine = StartCoroutine(AppearThreadCoroutine());
    }
    private IEnumerator AppearThreadCoroutine()
    {
        yield return new WaitForSeconds(_respawnTime);
        AppearThread();
    }

    public void Enter(GameObject go)
    {
        _isHit = true;
        GameObject player = go;
        if (!_playersOnRope.Contains(player))
        {
            _playersOnRope.Add(player);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            _initGravity = rb.gravityScale;
            _initDamping = rb.linearDamping;

            rb.gravityScale = 0f;
            rb.linearDamping = _damping;
        }
        if (_disappearCoroutine == null)
        {
            _disappearCoroutine = StartCoroutine(DisappearThreadCoroutine());
        }
    }

    public void Exit(GameObject go)
    {
        _isHit = false;
        GameObject player = go;
        if (_playersOnRope.Contains(player))
        {
            _playersOnRope.Remove(player);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            rb.gravityScale = _initGravity;
            rb.linearDamping = _initDamping;
        }
    }
}
