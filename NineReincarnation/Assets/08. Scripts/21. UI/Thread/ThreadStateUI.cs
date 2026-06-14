using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ✨ 추가: 스프라이트 세트를 묶어서 관리하기 위한 클래스
[System.Serializable]
public class ThreadUISpriteSet
{
    public Sprite step1Sprite; // ~30%
    public Sprite step2Sprite; // ~70%
    public Sprite step3Sprite; // 70%~
}

public class ThreadStateUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private ThrowThread throwThread; // ThrowRedThread 대신 부모 클래스 혹은 해당 스크립트 타입 사용
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressImage;

    [Header("오브젝트별 UI 스프라이트 설정")]
    [SerializeField] private ThreadUISpriteSet defaultSprites; // 기본 (혹은 다른 오브젝트용)
    [SerializeField] private ThreadUISpriteSet featherSprites; // 깃털 연결 시
    [SerializeField] private ThreadUISpriteSet catSprites;     // 고양이 연결 시

    [Header("설정")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

    // 현재 사용 중인 스프라이트 세트를 저장할 변수
    private ThreadUISpriteSet _currentSprites;

    private void OnEnable()
    {
        // ThrowThread 스크립트 구조에 따라 이벤트 이름이 다를 수 있으니 맞게 맞춰주세요.
        throwThread.OnConnected += ShowUI;
        // throwThread.OnDistanceUpdate += UpdateUI; // (기존 코드 유지)
        throwThread.OnDisconnected += HideUI;

        canvasGroup.alpha = 0f; // 초기 상태는 투명하게
    }

    private void OnDisable()
    {
        throwThread.OnConnected -= ShowUI;
        // throwThread.OnDistanceUpdate -= UpdateUI; // (기존 코드 유지)
        throwThread.OnDisconnected -= HideUI;
    }

    private void ShowUI()
    {
        // ✨ 추가: 타겟이 무엇인지 확인하고 알맞은 스프라이트 세트 할당
        if (throwThread.targetTransform != null)
        {
            if (throwThread.targetTransform.GetComponent<Feather>() != null)
            {
                _currentSprites = featherSprites;
            }
            else if (throwThread.targetTransform.GetComponent<Cat>() != null)
            {
                _currentSprites = catSprites;
            }
            else
            {
                _currentSprites = defaultSprites;
            }
        }
        else
        {
            _currentSprites = defaultSprites;
        }

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

        // 현재 선택된 스프라이트 세트가 없다면 리턴
        if (_currentSprites == null) return;

        // 비율에 따른 스프라이트 교체
        if (ratio < 0.3f) progressImage.sprite = _currentSprites.step1Sprite;
        else if (ratio < 0.6f) progressImage.sprite = _currentSprites.step2Sprite;
        else progressImage.sprite = _currentSprites.step3Sprite;
    }

    private void HideUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 0.5f)); 
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