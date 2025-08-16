using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("#Volmue")]
    [SerializeField] private SoundVolumeSO volumeData;
    public SoundVolumeSO VolumData => volumeData;

    [Header("#BGM")]
    public AudioClip[] BgmClips;
    public int BGMChannels;
    private AudioSource _bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] SfxClips;
    public int SFXChannels;
    private AudioSource[] _sfxPlayers;
    private int _sfxChannelIndex;

    [Header("#LoopingSFX")]
    public AudioClip[] LoopingSfxClips;
    public int LoopingChannels;
    private AudioSource[] _loopingSfxPlayers;
    private int _loopingChannelIndex;

    private GameObject _target = null;


    public enum Bgm
    {
        None = -1,
        Title,
        Stage
    }

    public enum Sfx
    {
        DIe,
        Jump,
        SavePoint,
        Zoom,
        Text,
        Click = 10
    }
    public enum LoopSfx
    {
        Walk
    }

    private void Awake()
    {
        Instance = this;
        SceneEventHandler.SceneStarted += SceneEvent_SetTarget;
        SceneEventHandler.SceneExited += SceneEvent_ClearTarget;

        SoundEventHandler.OnUpdateSfxVolmue += UpdateSfxVolmue;
        SoundEventHandler.OnUpdateBgmVolmue += UpdateBgmVolmue;
        Init();
    }

    private void OnDestroy()
    {
        SceneEventHandler.SceneStarted -= SceneEvent_SetTarget;
        SceneEventHandler.SceneExited -= SceneEvent_ClearTarget;

        SoundEventHandler.OnUpdateSfxVolmue -= UpdateSfxVolmue;
        SoundEventHandler.OnUpdateBgmVolmue -= UpdateBgmVolmue;
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            transform.position = _target.transform.position;
        }
    }

    private void SceneEvent_SetTarget()
    {
        _target = Camera.main.gameObject;
    }

    private void SceneEvent_ClearTarget()
    {
        _target = null;
        transform.position = Vector3.zero;
    }

    private void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = volumeData.BgmVolume;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        _sfxPlayers = new AudioSource[SFXChannels];

        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            _sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            _sfxPlayers[index].playOnAwake = false;
            _sfxPlayers[index].volume = volumeData.SfxVolume;
        }

        //반복 재생이 필요한 효과음 플레이어 초기화
        GameObject loopingSfxObject = new GameObject("LoopingSfxPlayer");
        loopingSfxObject.transform.parent = transform;
        _loopingSfxPlayers = new AudioSource[LoopingChannels];

        for (int index = 0; index < _loopingSfxPlayers.Length; index++)
        {
            _loopingSfxPlayers[index] = loopingSfxObject.AddComponent<AudioSource>();
            _loopingSfxPlayers[index].playOnAwake = false;
            _loopingSfxPlayers[index].volume = volumeData.SfxVolume;
            _loopingSfxPlayers[index].loop = true;
        }
    }

    private void UpdateSfxVolmue(float volume)
    {
        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            _sfxPlayers[index].volume = volume;
        }

        for (int index = 0; index < _loopingSfxPlayers.Length; index++)
        {
            _loopingSfxPlayers[index].volume = volume;
        }
    }

    private void UpdateBgmVolmue(float volume)
    {
        _bgmPlayer.volume = volume;
    }


    public void PlayBgm(Bgm bgm)
    {
        _bgmPlayer.Stop();


        if (bgm == Bgm.None) return;

        _bgmPlayer.clip = BgmClips[(int)bgm];

        // 새로운 BGM 재생
        _bgmPlayer.Play();
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
            int loopIndex = (index + _sfxChannelIndex) % _sfxPlayers.Length;

            if (_sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            _sfxChannelIndex = loopIndex;

            _sfxPlayers[loopIndex].clip = SfxClips[(int)sfx];
            _sfxPlayers[loopIndex].Play();

            break;
        }
    }
}
