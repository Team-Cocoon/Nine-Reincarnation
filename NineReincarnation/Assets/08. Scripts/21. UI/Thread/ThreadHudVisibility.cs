using UnityEngine;

// 실(청연/홍연) HUD 전체를 스토리 진행 중에는 숨기고, 끝나면 다시 보이게 한다.
// DialogueManager가 UIEventHandler.OnStoryPlayingChanged 로 상태를 알려준다.
//
// 세팅: 숨길 HUD 루트에 CanvasGroup을 붙이고 여기에 연결.
//  - SetActive 대신 CanvasGroup을 쓰는 이유: 오브젝트가 비활성화되면 이 스크립트도 멈춰
//    다시 켜는 이벤트를 못 받기 때문. alpha만 0으로 만들어 표시만 끈다.
public class ThreadHudVisibility : MonoBehaviour
{
    [Header("--- 숨길 HUD 루트 ---")]
    [SerializeField] private CanvasGroup _hudCanvasGroup;

    private void Awake()
    {
        if (_hudCanvasGroup == null) _hudCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        UIEventHandler.OnStoryPlayingChanged += HandleStoryPlaying;
        // 구독 전에 스토리가 이미 시작됐을 수 있으므로 현재 상태를 즉시 반영
        HandleStoryPlaying(UIEventHandler.IsStoryPlaying);
    }

    private void OnDisable()
    {
        UIEventHandler.OnStoryPlayingChanged -= HandleStoryPlaying;
    }

    private void HandleStoryPlaying(bool playing)
    {
        if (_hudCanvasGroup == null) return;

        _hudCanvasGroup.alpha = playing ? 0f : 1f;
        _hudCanvasGroup.interactable = !playing;
        _hudCanvasGroup.blocksRaycasts = !playing;
    }
}
