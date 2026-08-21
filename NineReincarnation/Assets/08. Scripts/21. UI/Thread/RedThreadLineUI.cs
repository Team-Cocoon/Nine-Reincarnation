using UnityEngine;
using UnityEngine.UI;

// 홍연(빨간 실) HUD 라인.
// UIEventHandler의 홍연 이벤트를 받아 라인 색/투명도를 제어한다.
//  - 비활성: 회색
//  - 연결: 빨강 + 연결 대상과의 거리 3단계 투명도
//  - 해제: 회색으로 부드럽게 복귀(≈0.5s 느낌)
//
// 실 셰이더가 UI 버텍스 컬러를 곱하는 구조(홍연 실이 LineRenderer 버텍스 컬러로 페이드됐던 것과 동일)이므로
// Image.color 로 색/알파를 함께 제어한다. → 라인 머티리얼의 _Color 는 흰색으로 두세요.
public class RedThreadLineUI : MonoBehaviour
{
    [Header("--- 참조 ---")]
    [SerializeField] private Graphic _lineGraphic;   // 파형 라인 Image

    [Header("--- 색 ---")]
    [SerializeField] private Color _activeColor = new Color(1f, 0.15f, 0.15f, 1f);   // 연결 시 빨강
    [SerializeField] private Color _inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 비활성 회색

    [Header("--- 거리 3단계 투명도 ---")]
    [Range(0f, 1f)] [SerializeField] private float _alphaStage1 = 1.0f;   // 여유 많음(가까움)
    [Range(0f, 1f)] [SerializeField] private float _alphaStage2 = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float _alphaStage3 = 0.3f;   // 끊기 직전(멀음)
    [Range(0f, 1f)] [SerializeField] private float _stage1Remaining = 0.6f; // 남은 여유 >= 0.6 → 1단계
    [Range(0f, 1f)] [SerializeField] private float _stage2Remaining = 0.3f; // >= 0.3 → 2단계, 그 미만 → 3단계

    [Header("--- 전환 속도 ---")]
    [Tooltip("색/투명도 전환 속도. 단계 전환과 해제 시 회색 복귀(≈0.5s 느낌)에 사용")]
    [SerializeField] private float _transitionSpeed = 8f;

    [Header("--- 좌측 매듭 아이콘 (셰이더 아님) ---")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _connectedIcon;    // 연결 시 빨간 매듭 이미지
    [SerializeField] private Sprite _disconnectedIcon; // 미연결 시 회색 실 이미지

    private bool _connected;
    private Color _targetColor;

    private void Awake()
    {
        if (_lineGraphic == null) _lineGraphic = GetComponent<Graphic>();
        _targetColor = _inactiveColor;
        if (_lineGraphic != null) _lineGraphic.color = _inactiveColor;

        SetIcon(_disconnectedIcon);   // 시작은 미연결(회색)
    }

    private void OnEnable()
    {
        UIEventHandler.OnRedThreadConnected += HandleConnected;
        UIEventHandler.OnRedThreadDistanceChanged += HandleDistance;
        UIEventHandler.OnRedThreadDisconnected += HandleDisconnected;
    }

    private void OnDisable()
    {
        UIEventHandler.OnRedThreadConnected -= HandleConnected;
        UIEventHandler.OnRedThreadDistanceChanged -= HandleDistance;
        UIEventHandler.OnRedThreadDisconnected -= HandleDisconnected;
    }

    private void HandleConnected()
    {
        _connected = true;
        SetTarget(_activeColor, _alphaStage1);
        SetIcon(_connectedIcon);   // 빨간 매듭
    }

    private void HandleDistance(float ratio)
    {
        if (!_connected) return;
        SetTarget(_activeColor, StageAlpha(ratio));
    }

    private void HandleDisconnected()
    {
        _connected = false;
        _targetColor = _inactiveColor;   // 회색 + 알파 1
        SetIcon(_disconnectedIcon);      // 회색 실
    }

    private void SetIcon(Sprite s)
    {
        if (_iconImage != null && s != null) _iconImage.sprite = s;
    }

    private void SetTarget(Color baseColor, float alpha)
    {
        _targetColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }

    // ratio: 0 = 가까움(여유 많음), 1 = 끊기 직전(멀음)
    private float StageAlpha(float ratio)
    {
        float remaining = 1f - Mathf.Clamp01(ratio);   // 1 = 가까움, 0 = 끊기 직전
        if (remaining >= _stage1Remaining) return _alphaStage1;
        if (remaining >= _stage2Remaining) return _alphaStage2;
        return _alphaStage3;
    }

    private void Update()
    {
        if (_lineGraphic == null) return;

        _lineGraphic.color = Color.Lerp(
            _lineGraphic.color, _targetColor,
            Mathf.Clamp01(_transitionSpeed * Time.deltaTime));
    }
}
