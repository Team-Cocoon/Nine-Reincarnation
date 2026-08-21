using System.Collections.Generic;
using UnityEngine;

public enum BubbleImageType
{
    None = -1,
    Normal = 0,
    Round = 1,
    Spiky = 2
}

[CreateAssetMenu(fileName = "BubbleDataSO", menuName = "Scriptable Objects/BubbleDataSO")]
public class BubbleDataSO : ScriptableObject
{
    [SerializeField] private List<Sprite> _sprites;

    public Sprite GetSprite(int index)
    {
        return _sprites[index];
    }
}
