using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.Controller;

// 청연(파란 실) HUD 게이지 — 최대 개수에 맞춰 칸을 자동 생성/제거.
//  - MaxBlueThread(최대치)에 맞춰 칸(_segmentPrefab)을 복제해 개수를 맞춘다. (IncreaseMaxBlueThread 대응)
//  - 현재 개수만큼 칸을 채우고, 회복 중인 경계 칸만 회복 진행도(0~1)로 부분 채움.
//  - 사용 시 해당 칸이 _drainSpeed 로 줄어든다.
//  - 좌측 매듭 아이콘: 개수 0이면 회색 실 이미지, 1개라도 있으면 원래 이미지.
//
// 세팅:
//  - _segmentPrefab : 칸 1개 템플릿 Image (씬에 두고, Image Type = Filled/Horizontal/Left, 파형 머티리얼).
//    Awake에서 자동으로 비활성(SetActive false) 처리되어 복제용 원본으로만 쓰인다.
//  - _segmentParent : 칸들이 들어갈 부모. HorizontalLayoutGroup(Child Force Expand Width)로 칸이 균등 배치되게.
public class BlueThreadGaugeUI : MonoBehaviour
{
    [Header("--- 칸 자동 생성 ---")]
    [Tooltip("칸 1개 템플릿 (Filled/Horizontal/Left + 파형 머티리얼). 씬에 두면 자동 비활성화되어 복제 원본으로 쓰임")]
    [SerializeField] private Image _segmentPrefab;
    [Tooltip("칸들이 들어갈 부모. HorizontalLayoutGroup 권장")]
    [SerializeField] private Transform _segmentParent;

    [Header("--- 채움/줄어듦 속도 (fillAmount/초) ---")]
    [Tooltip("회복 추종 속도. 실제 5초 회복 타이머를 부드럽게 따라가도록 충분히 크게")]
    [SerializeField] private float _refillSpeed = 8f;
    [Tooltip("사용 시 줄어드는 속도. 낮추면 천천히 줄어듦")]
    [SerializeField] private float _drainSpeed = 8f;

    [Header("--- 좌측 매듭 아이콘 (셰이더 아님) ---")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _activeIcon;    // 개수가 남아있을 때(원래 이미지)
    [SerializeField] private Sprite _inactiveIcon;  // 모두 소진(0개) 시 회색 실 이미지

    private readonly List<Image> _segments = new List<Image>();
    private readonly List<float> _displayed = new List<float>();
    private int _lastIconState = -1; // -1:미설정, 0:비활성, 1:활성

    private PlayerController Player =>
        (InputManager.Instance != null) ? InputManager.Instance.CurPlayer : null;

    private void Awake()
    {
        // 템플릿은 복제 원본으로만 사용 → 화면에서 숨긴다.
        if (_segmentPrefab != null) _segmentPrefab.gameObject.SetActive(false);
    }

    private void Update()
    {
        PlayerController player = Player;
        if (player == null) return;

        EnsureSegmentCount(Mathf.Max(0, player.MaxBlueThread));
        UpdateSegments(player.BlueThread, player.BlueRecoverProgress01);
        UpdateIcon(player.BlueThread);
    }

    // 최대치에 맞춰 칸 개수를 늘리거나 줄인다.
    private void EnsureSegmentCount(int max)
    {
        // 파괴된(null) 칸 정리
        for (int i = _segments.Count - 1; i >= 0; i--)
        {
            if (_segments[i] == null)
            {
                _segments.RemoveAt(i);
                _displayed.RemoveAt(i);
            }
        }

        if (_segmentPrefab == null || _segmentParent == null) return;

        // 부족하면 복제 생성
        while (_segments.Count < max)
        {
            Image seg = Instantiate(_segmentPrefab, _segmentParent);
            seg.gameObject.SetActive(true);
            seg.fillAmount = 0f;
            _segments.Add(seg);
            _displayed.Add(0f);
        }

        // 남으면 제거
        while (_segments.Count > max)
        {
            int last = _segments.Count - 1;
            if (_segments[last] != null) Destroy(_segments[last].gameObject);
            _segments.RemoveAt(last);
            _displayed.RemoveAt(last);
        }
    }

    private void UpdateSegments(int count, float recover)
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            if (_segments[i] == null) continue;

            float target;
            if (i < count) target = 1f;            // 채워진 칸
            else if (i == count) target = recover; // 회복 중인 칸(부분 채움)
            else target = 0f;                      // 빈 칸

            float speed = (target >= _displayed[i]) ? _refillSpeed : _drainSpeed;
            _displayed[i] = Mathf.MoveTowards(_displayed[i], target, speed * Time.deltaTime);
            _segments[i].fillAmount = _displayed[i];
        }
    }

    private void UpdateIcon(int count)
    {
        if (_iconImage == null) return;

        int state = (count > 0) ? 1 : 0;
        if (state == _lastIconState) return;   // 상태 바뀔 때만 교체
        _lastIconState = state;

        Sprite s = (state == 1) ? _activeIcon : _inactiveIcon;
        if (s != null) _iconImage.sprite = s;
    }
}
