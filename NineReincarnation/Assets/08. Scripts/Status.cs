using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "StatData")]
public class Status : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private StatusData _statData;

    public float Speed => _statData.speed;
    public float JumpPower => _statData.jumpPower;

    public void Initialize(StatusData data)
    {
        _statData = data;
    }
}

[System.Serializable]
public struct StatusData
{
    public float speed;
    public float jumpPower;

    public StatusData(float speed, float jumpPower)
    {
        this.speed = speed;
        this.jumpPower = jumpPower;
    }
}
