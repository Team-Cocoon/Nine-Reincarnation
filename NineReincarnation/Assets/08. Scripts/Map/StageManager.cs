using UnityEngine;
using Utilities;

public class StageManager : MonoBehaviour
{
    [Header("--- 스테이지 모음 ---")]
    [SerializeField] private Stage[] _stages;

    private void Start()
    {
        if(_stages.Length >= 1)
        {
            _stages[0].LoadContent();
        }
    }

    private void StageLoad()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
