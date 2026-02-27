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
