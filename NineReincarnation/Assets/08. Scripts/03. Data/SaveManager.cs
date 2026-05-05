using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("---- Save Data ----")]
    [SerializeField] private SaveDataSO[] _saveDatas;
    [SerializeField] private bool _isTest;

    private SaveDataSO _curSaveData;
    public GameProgressData GameData;

    public void SetSaveData(int index)
    {
        // 방어 코드: 인덱스가 배열 범위를 벗어나지 않도록
        if (index < 0 || index >= _saveDatas.Length) return;

        if (_isTest)
        {
            DataClear(index);
        }

        _curSaveData = _saveDatas[index];
        if (GameData != null)
        {
            GameData = null;
        }
        GameData = new GameProgressData(_curSaveData);
    }

    public void Save()
    {
        // 🌟 [추가됨] 타이틀 씬을 거치지 않은 에디터 다이렉트 테스트를 위한 예외 처리
        if (_curSaveData == null)
        {
            Debug.LogWarning("[SaveManager] 타이틀 씬을 거치지 않아 데이터가 없습니다. 테스트를 위해 0번 슬롯을 강제로 로드합니다.");
            
            // saveDatas 배열이 비어있지 않다면 0번 인덱스를 할당
            if (_saveDatas != null && _saveDatas.Length > 0)
            {
                SetSaveData(0); 
            }
            else
            {
                Debug.LogError("[SaveManager] SaveDataSO 배열이 인스펙터에 할당되어 있지 않습니다!");
                return; // 저장 포기
            }
        }

        _curSaveData.Save(GameData);
    }

    public void SetState()
    {
        //GameData.State = state;
    }

    private void DataClear(int index)
    {
        _saveDatas[index].Init();
    }
}