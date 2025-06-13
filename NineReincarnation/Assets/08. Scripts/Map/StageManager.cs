using UnityEngine;
using Utilities;

public class StageManager : MonoBehaviour
{
    [Header("--- 스테이지 모음 ---")]
    [SerializeField] private Stage[] _stages;

    private void Start()
    {
        _stages[0].LoadContent();
    }
}
