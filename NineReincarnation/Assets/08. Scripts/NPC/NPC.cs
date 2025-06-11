using System;
using UnityEngine;

public interface IEvent
{
    public string objName { get; }
    /// <summary>
    /// 애니메이션 재생
    /// </summary>
    /// <param name="eventName"></param>
    public void StartAnim(string animName);
    /// <summary>
    /// 콜백 세팅
    /// </summary>
    /// <param name="eventAction"></param>
    public void SettingAnimEvent(Action eventAction);
    /// <summary>
    /// 애니메이션 특정 프레임에서 호출
    /// </summary>
    /// <param name="animName"></param>
    public void AnimEvent(string animName);
    /// <summary>
    /// 특정 이벤트 호출할 때
    /// </summary>
    /// <param name="eventName"></param>
    public void TriggerEvent(string eventName, Action triggerAction = null);
}

public class NPC : MonoBehaviour, IEvent
{
    [SerializeField] private string _objName; // 캐릭터 이름
    
    protected Animator _animator;
    protected Action _animAction;
    protected Action _triggerAction;
    protected string _currentAnimName;

    public string objName => _objName;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public virtual void StartAnim(string animName)
    {
        throw new System.NotImplementedException();
    }
    public virtual void SettingAnimEvent(Action eventAction)
    {
        _animAction = eventAction;
    }
    public virtual void AnimEvent(string animName)
    {
        if (_currentAnimName == animName)
        {
            var action = _animAction;  
            _animAction = null;        
            action?.Invoke();         
        }
    }
    public virtual void TriggerEvent(string eventName, Action triggerAction)
    {
        throw new System.NotImplementedException();
    }
}