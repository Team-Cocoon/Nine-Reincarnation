using UnityEngine;

[CreateAssetMenu(fileName = "SoundVolmueSO", menuName = "Scriptable Objects/SoundVolmueSO")]
public class SoundVolumeSO : ScriptableObject
{
    [SerializeField] private float _sfxVolume;
    [SerializeField] private float _bgmVolume;

    public float SfxVolume
    {
        get { return _sfxVolume; }
        set
        {
            _sfxVolume = value;
            SoundEventHandler.OnUpdateSfxVolmue_Invoke(_sfxVolume);
        }
    }

    public float BgmVolume
    {
        get { return _bgmVolume; }
        set
        {
            _bgmVolume = value;
            SoundEventHandler.OnUpdateSfxVolmue_Invoke(_bgmVolume);
        }
    }
}
