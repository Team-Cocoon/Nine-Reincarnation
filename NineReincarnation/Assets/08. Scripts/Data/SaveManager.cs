using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("---- Save Data ----")]
    [SerializeField] private SaveDataSO[] _saveData;

    public SaveDataSO SaveData;

    public void SetSaveData(int index)
    {
        SaveData = _saveData[index];
    }
    private void DataClear(int index)
    {
        _saveData[index].Init();
    }
}
