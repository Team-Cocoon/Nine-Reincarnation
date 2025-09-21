using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundVolmueSO", menuName = "Scriptable Objects/SoundVolmueSO")]
public class SoundVolumeSO : ScriptableObject
{
    public float MasterVolume;
    public float SfxVolume;
    public float BgmVolume;
}
