using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public static AudioManger Instance { get; private set; }

    [Header("#BGM")]
    public AudioClip BgmClip;
    public float BgmVolume;
    private AudioSource _bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] SfxClips;
    public float SfxVolume;
    public int Chnnels;
    private AudioSource[] _sfxPlayers;
    private int _channelIndex;

    [Header("#LoopingSFX")]
    public AudioClip[] LoopingSfxClips;
    public float LoopingSfxVolume;
    public int LoopingChnnels;
    private AudioSource[] _loopingSfxPlayers;
    private int _loopingChannelIndex;

    public enum Sfx
    {
        DIe,
        Landing,
        Jump,
        SavePoint,
        Zoom
    }
    public enum LoopSfx
    {
        Walk
    }

    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Start()
    {
        PlayBgm(true);
    }

    private void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = BgmVolume;
        _bgmPlayer.clip = BgmClip;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        _sfxPlayers = new AudioSource[Chnnels];

        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            _sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            _sfxPlayers[index].playOnAwake = false;
            _sfxPlayers[index].volume = SfxVolume;
        }

        //반복 재생이 필요한 효과음 플레이어 초기화
        GameObject loopingSfxObject = new GameObject("LoopingSfxPlayer");
        loopingSfxObject.transform.parent = transform;
        _loopingSfxPlayers = new AudioSource[LoopingChnnels];

        for (int index = 0; index < _loopingSfxPlayers.Length; index++)
        {
            _loopingSfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            _loopingSfxPlayers[index].playOnAwake = false;
            _loopingSfxPlayers[index].volume = SfxVolume;
            _loopingSfxPlayers[index].loop = true;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            _bgmPlayer.Play();
        }
        else
        {
            _bgmPlayer.Stop();
        }
    }

    public void PlayLoopingSfx(LoopSfx sfx)
    {
        for (int index = 0; index < _loopingSfxPlayers.Length; index++)
        {
            int loopIndex = (index + _loopingChannelIndex) % _loopingSfxPlayers.Length;

            if (_loopingSfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            _loopingChannelIndex = loopIndex;

            _loopingSfxPlayers[loopIndex].clip = LoopingSfxClips[(int)sfx];
            _loopingSfxPlayers[loopIndex].Play();

            break;
        }
    }

    public void StopLoopingSfx(LoopSfx sfx)
    {
        AudioClip targetClip = _loopingSfxPlayers[(int)sfx].clip;

        for (int index = 0; index < _loopingSfxPlayers.Length; index++)
        {
            int loopIndex = (index + _loopingChannelIndex) % _loopingSfxPlayers.Length;

            AudioSource player = _loopingSfxPlayers[loopIndex];

            if (player.isPlaying && player.clip == targetClip)
            {
                player.Stop();
                break;
            }
        }
    }


    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            int loopIndex = (index + _channelIndex) % _sfxPlayers.Length;

            if (_sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            _channelIndex = loopIndex;

            _sfxPlayers[loopIndex].clip = SfxClips[(int)sfx];
            _sfxPlayers[loopIndex].Play();

            break;
        }
    }
}
