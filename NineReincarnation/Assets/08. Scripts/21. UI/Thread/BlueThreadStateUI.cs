using System.Collections.Generic;
using UnityEngine;
using EventHandler;

public class BlueThreadStateUI : MonoBehaviour
{
    [Header("--- 프리팹(템플릿) 및 부모 ---")]
    [SerializeField] private BlueThreadSlotUI _slotPrefab; 
    [SerializeField] private Transform _layoutGroupParent;

    [Header("--- 실 이미지 ---")]
    [SerializeField] private Sprite _activeThreadSprite;
    [SerializeField] private Sprite _inactiveThreadSprite;

    [Header("--- 연결고리 이미지 ---")]
    [SerializeField] private Sprite _connectedRingSprite;
    [SerializeField] private Sprite _disconnectedRingSprite;

    private List<BlueThreadSlotUI> _spawnedSlots = new List<BlueThreadSlotUI>();

    private void OnEnable()
    {
        UIEventHandler.OnMaxBlueThreadChanged += InitOrUpdateSlots;
        UIEventHandler.OnBlueThreadCountChanged += UpdateThreadCountUI;
    }

    private void OnDisable()
    {
        UIEventHandler.OnMaxBlueThreadChanged -= InitOrUpdateSlots;
        UIEventHandler.OnBlueThreadCountChanged -= UpdateThreadCountUI;
    }

    private void InitOrUpdateSlots(int maxCount)
    {
        // [안전장치 1] 원본 템플릿 자체가 파괴되었는지 검사
        if (_slotPrefab == null)
        {
            Debug.LogError("에러: 복사할 원본(_slotPrefab)이 파괴되어 없습니다! 사망 시 UI를 Destroy하는 로직이 원본까지 지웠는지 확인하세요.");
            return;
        }

        _slotPrefab.gameObject.SetActive(false);

        // [안전장치 2] 리스트 안에 들어있던 슬롯 중, 사망 이벤트 등으로 인해 강제 파괴된(null) 녀석들이 있다면 먼저 청소
        _spawnedSlots.RemoveAll(slot => slot == null);

        // 1. 부족한 만큼 복사본 생성
        while (_spawnedSlots.Count < maxCount)
        {
            BlueThreadSlotUI newSlot = Instantiate(_slotPrefab, _layoutGroupParent);
            newSlot.gameObject.SetActive(true);
            _spawnedSlots.Add(newSlot);
        }

        // 2. 남는 만큼 삭제
        while (_spawnedSlots.Count > maxCount)
        {
            int lastIndex = _spawnedSlots.Count - 1;
            if (_spawnedSlots[lastIndex] != null)
            {
                Destroy(_spawnedSlots[lastIndex].gameObject);
            }
            _spawnedSlots.RemoveAt(lastIndex);
        }

        // 3. 마지막 슬롯의 연결고리는 숨김 처리
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (_spawnedSlots[i] == null) continue; // 2차 안전장치

            bool isLastSlot = (i == _spawnedSlots.Count - 1);
            _spawnedSlots[i].SetConnectionVisibility(!isLastSlot);
        }
    }

    private void UpdateThreadCountUI(int currentCount)
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            bool isActive = (i < currentCount);

            _spawnedSlots[i].SetThreadState(isActive, _activeThreadSprite, _inactiveThreadSprite);
            _spawnedSlots[i].SetConnectionState(isActive, _connectedRingSprite, _disconnectedRingSprite);
        }
    }
}