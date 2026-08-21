using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRequirementSO", menuName = "Scriptable Objects/SceneRequirementSO")]
public class SceneRequirementSO : ScriptableObject
{
    [Serializable]
    public class Rule
    {
        public string sceneName;
        public List<string> requiredScenes = new();
    }

    [SerializeField] private List<Rule> rules = new();

    public IReadOnlyList<string> GetRequiredScenes(string sceneName)
    {
        for (int i = 0; i < rules.Count; i++)
        {
            if (rules[i].sceneName == sceneName)
            {
                return rules[i].requiredScenes;
            }
        }

        return Array.Empty<string>();
    }
    
}
