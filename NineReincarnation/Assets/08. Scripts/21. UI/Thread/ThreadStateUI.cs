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

    private void OnEnable()
    {
        throwThread.OnConnected += ShowUI;
        throwThread.OnDistanceUpdate += UpdateUI;
        throwThread.OnDisconnected += HideUI;

        canvasGroup.alpha = 0f; // 초기 상태는 투명하게
    }

    private void OnDisable()
    {
        throwThread.OnConnected -= ShowUI;
        throwThread.OnDistanceUpdate -= UpdateUI;
        throwThread.OnDisconnected -= HideUI;
    }

    private void ShowUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0.3f));
    }

    private void UpdateUI(float ratio)
    {
        // 위치 업데이트 (타겟 머리 위)
        if (throwThread.targetTransform != null)
        {
            transform.position = throwThread.targetTransform.position + offset;
        }

        // 비율에 따른 스프라이트 교체
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
