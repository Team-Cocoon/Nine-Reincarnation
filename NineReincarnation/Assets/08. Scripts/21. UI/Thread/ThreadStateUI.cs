using System.Collections;

using UnityEngine;

using UnityEngine.UI;



public class ThreadStateUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private ThrowRedThread throwThread; // 연결된 로직 스크립트
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressImage;

    [Header("설정 및 리소스")]
    [SerializeField] private Sprite step1Sprite; // ~30%
    [SerializeField] private Sprite step2Sprite; // ~70%
    [SerializeField] private Sprite step3Sprite; // 70%~
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("UI 상태")]
    [SerializeField] private Image _interactionSprite; // 상호작용하는 이미지
    [SerializeField] private Sprite _catSprite; // 고양이
    [SerializeField] private Sprite _featherSprite; // 닭

    private void OnEnable()
    {
        // 참조가 파괴/미할당 상태면 접근하지 않는다(플레이 종료 시 파괴 순서 문제 방지).
        if (throwThread != null)
        {
            throwThread.OnConnected += ShowUI;
            throwThread.OnDistanceUpdate += UpdateUI;
            throwThread.OnDisconnected += HideUI;
        }

        if (canvasGroup != null) canvasGroup.alpha = 0f; // 초기 상태는 투명하게
    }

    private void OnDisable()
    {
        // ThrowRedThread가 이 UI보다 먼저 파괴되면 MissingReferenceException이 나므로 null 체크.
        if (throwThread != null)
        {
            throwThread.OnConnected -= ShowUI;
            throwThread.OnDistanceUpdate -= UpdateUI;
            throwThread.OnDisconnected -= HideUI;
        }
    }

    private void ShowUI()
    {
        if (throwThread != null && throwThread.targetTransform != null && _interactionSprite != null)
        {
            if (throwThread.targetTransform.GetComponent<Feather>() != null)
            {
                _interactionSprite.sprite = _featherSprite;
            }
            else if (throwThread.targetTransform.GetComponent<Cat>() != null)
            {
                _interactionSprite.sprite = _catSprite;
            }
        }
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0.3f));
    }

    private void UpdateUI(float ratio)
    {
        // 위치 업데이트 (타겟 머리 위)
        if (throwThread != null && throwThread.targetTransform != null)
        {
            transform.position = throwThread.targetTransform.position + offset;
        }

        // 비율에 따른 스프라이트 교체
        if (progressImage == null) return;
        if (ratio < 0.3f) progressImage.sprite = step1Sprite;
        else if (ratio < 0.6f) progressImage.sprite = step2Sprite;
        else progressImage.sprite = step3Sprite;
    }

    private void HideUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 0.5f)); // 요청하신 1초 Fade Out
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}

