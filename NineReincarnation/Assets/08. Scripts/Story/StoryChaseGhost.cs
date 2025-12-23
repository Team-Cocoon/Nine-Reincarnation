using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryChaseGhost : StoryNPC, IEventInterface
{
    [SerializeField] private RuntimeAnimatorController _storyAnimator;
    [SerializeField] private RuntimeAnimatorController _defaultAnimator;
    [SerializeField] private AudioSource _audioSource;
    private ChaseGhost _chaseGhost;

    public async UniTask ExecuteEvent(int index)
    {
        NpcAnimator.runtimeAnimatorController = _defaultAnimator;
        _chaseGhost.StartBehavior();
    }

    public void SoundPlay()
    {
        _audioSource.Play();
    }

    public void AppearedSoundPlay()
    {
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.Chase);
    }

    private void Start()
    {
        _chaseGhost = GetComponent<ChaseGhost>();
        _chaseGhost.EndBehavior();
        NpcAnimator.runtimeAnimatorController = _storyAnimator;
    }
}
