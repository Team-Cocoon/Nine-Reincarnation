using ExcelData;
using Febucci.UI.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryAnimationManager : MonoBehaviour
{
    [Header("----Animator-----")]
    [SerializeField] private Animator[] _animators;

    private Dictionary<string, Animator> _animatorDict = new();

    private void Awake()
    {
        foreach (Animator animator in _animators)
        {
            _animatorDict.Add(animator.gameObject.name, animator);
        }
    }
    
    public void ExcuteAnimation(AnimationClass animationData)
    {
        string name = animationData.Name;
        string animationName = animationData.AnimationName;
        float duration = animationData.Duration;

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(animationName))
        {
            _animatorDict[name].Play(animationName);
        }
    }
}
