using UnityEngine;

public class MainTitleAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject titleMenuButton;

    /// <summary>
    /// [새로 추가] 루프 애니메이션이 처음 시작될 때 버튼을 활성화하는 함수
    /// </summary>
    public void ShowMenuButton()
    {
        if (titleMenuButton != null)
        {
            // 현업 팁: 이미 켜져 있다면 SetActive를 중복 호출하지 않도록 방어 코드를 넣습니다.
            if (!titleMenuButton.activeSelf)
            {
                titleMenuButton.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("[MainTitle] 활성화할 버튼 오브젝트가 등록되지 않았습니다.");
        }
    }

    /// <summary>
    /// 메인 타이틀 글자가 나타나는 프레임에서 호출
    /// </summary>
    public void PlayMainTitleReveal()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainTitleReveal);
        }
    }

    /// <summary>
    /// 타이틀 글자가 반짝(Glint)이는 프레임에서 호출
    /// </summary>
    public void PlayMainGlint()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainGlint);
        }
    }

    /// <summary>
    /// 화면 전체 플래시가 터진 후 서서히 사라지는 프레임에서 호출
    /// </summary>
    public void PlayMainFlashFade()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MainFlashFade);
        }
    }
}