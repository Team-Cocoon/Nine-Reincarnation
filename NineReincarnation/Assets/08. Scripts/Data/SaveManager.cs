using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }

    [Header("---- 세이브 데이터 ----")]
    [SerializeField] private SaveDataSO[] _saveData;

    public SaveDataSO SaveData;

    public void SetSaveData(int index)
    {
        SaveData = _saveData[index];
    }

    public void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DataClear(int index)
    {
        _saveData[index].Init();
    }
}
