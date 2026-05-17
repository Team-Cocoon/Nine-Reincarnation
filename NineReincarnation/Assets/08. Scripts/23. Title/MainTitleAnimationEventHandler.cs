using DG.Tweening;
using UnityEngine;

public class MainTitleAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuButtonGroup;
    private void Awake()
    {
        // 게임 시작 시 버튼 그룹을 미리 숨겨둡니다.
        if (menuButtonGroup != null)
        {
            menuButtonGroup.alpha = 0f;
            menuButtonGroup.gameObject.SetActive(false);
        }
    }
    public void ShowMenuButton()
    {
        if (menuButtonGroup != null)
        {
            menuButtonGroup.DOKill();

            menuButtonGroup.gameObject.SetActive(true);

            float fadeDuration = 1.5f;

            menuButtonGroup.DOFade(1f, fadeDuration)
                .SetEase(Ease.Linear) 
                .SetLink(menuButtonGroup.gameObject);
        }
        else
        {
            Debug.LogWarning("[MainTitle] 활성화할 menuButtonGroup(CanvasGroup)이 등록되지 않았습니다.");
        }
    }

    public void PlayMainTitleReveal()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainTitleReveal);
        }
    }

    public void PlayMainGlint()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainGlint);
        }
    }

    public void PlayMainFlashFade()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainFlashFade);
        }
    }
}