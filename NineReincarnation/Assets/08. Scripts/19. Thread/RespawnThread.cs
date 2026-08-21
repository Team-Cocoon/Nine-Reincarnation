using System.Collections;
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
    [SerializeField] private float _margin = 0.5f;

    [Header("Sound")]
    [SerializeField] AudioSource _source;

    //private List<GameObject> _playersOnRope = new List<GameObject>();
    private Coroutine _disappearCoroutine;
    private Vector2[] _colliderPositions;

    protected override void UpdateThread()
    {
        UpdateSegments();
    }
    protected override void Initialize()
    {
        base.Initialize();
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _colliderPositions = new Vector2[segmentCount];
    }
    protected override void RenderThread()
    {
        int activeCount = GetActiveSegmentCount();
        if (activeCount <= 0 || _lineRenderer == null || _edgeCollider == null) return;

        segmentCount = activeCount;
        if (_colliderPositions == null || _colliderPositions.Length != activeCount)
        {
            _colliderPositions = new Vector2[activeCount];
        }

        _lineRenderer.startWidth = _lineRenderer.endWidth = threadWidth;
        _lineRenderer.positionCount = activeCount;
        for (int i = 0; i < activeCount; i++)
        {
            _segmentPositions[i] = segments[i].position;
            _colliderPositions[i] = transform.InverseTransformPoint(segments[i].position);
            _lineRenderer.SetPosition(i, _segmentPositions[i]);
        }

        _edgeCollider.edgeRadius = threadWidth * _margin;
        _edgeCollider.points = _colliderPositions;
    }
    /// <summary>
    /// 실의 충돌 시 위치 보정
    /// </summary>
    //private void AdjustCollision()
    //{
    //    for (int i = 0; i < segments.Count; i++)
    //    {
    //        Vector2 dir = segments[i].position - segments[i].prevPosition;

    //        RaycastHit2D hit = Physics2D.CircleCast(segments[i].position, threadWidth, dir.normalized, 0f, _hitLayer);

    //        if (hit) // 충돌 시 위치 보정
    //        {
    //            segments[i].position = hit.point + hit.normal * threadWidth * 0.5f;
    //            segments[i].prevPosition = segments[i].position;
    //        }
    //    }
    //}
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

        bool isSound = true;
        while (elapsedTime < 1f)
        {
            //if (!_isHit)
            //{
            //    AppearThread();
            //    yield break;  // 중간에 빠졌으면 중단
            //}
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / 1f);

            _lineRenderer.startColor = currentColor;
            _lineRenderer.endColor = currentColor;

            if (elapsedTime >= 0.8f && isSound)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.DeleteThread);
                isSound = false;
            }
            yield return null;
        }

        _lineRenderer.startColor = endColor;
        _lineRenderer.endColor = endColor;

        _lineRenderer.enabled = false;
        _edgeCollider.enabled = false;

        yield return StartCoroutine(AppearThreadCoroutine());
    }
    private IEnumerator AppearThreadCoroutine()
    {
        yield return new WaitForSeconds(_respawnTime);
        AppearThread();
    }

    public void Enter(GameObject go)
    {
        _isHit = true;

        if (_disappearCoroutine == null)
        {
            _source.Play();
            _disappearCoroutine = StartCoroutine(DisappearThreadCoroutine());
        }
    }

    public void Exit(GameObject go)
    {
        _source.Stop();
        _isHit = false;
    }
}
