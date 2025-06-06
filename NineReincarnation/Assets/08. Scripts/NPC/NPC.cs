using UnityEngine;

public interface IEvent
{
    public string objName { get; }
    public void TriggerEvent(string eventName);
}

public class NPC : MonoBehaviour, IEvent
{
    [SerializeField] private string _objName; // 캐릭터 이름
    public string objName => _objName;
    public void TriggerEvent(string eventName)
    {
        throw new System.NotImplementedException();
    }
}
