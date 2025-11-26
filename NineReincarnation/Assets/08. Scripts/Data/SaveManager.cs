using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("---- Save Data ----")]
    [SerializeField] private SaveDataSO[] _saveDatas;
    [SerializeField] private bool _isTest;

    private SaveDataSO _saveData;
    public GameProgressData GameData;

    public void SetSaveData(int index)
    {
        if (_isTest)
        {
            DataClear(index);
        }

        _saveData = _saveDatas[index];
        if (GameData != null)
        {
            GameData = null;
        }
        GameData = new GameProgressData(_saveData);
    }

    public void Save()
    {
        _saveData.Save(GameData);
    }

    public void SetState(GameState state)
    {
        GameData.State = state;
    }

    private void DataClear(int index)
    {
        _saveDatas[index].Init();
    }
}
