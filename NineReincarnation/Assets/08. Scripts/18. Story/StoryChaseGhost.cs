using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryChaseGhost : StoryNPC, IEventInterface
{
    [SerializeField] private RuntimeAnimatorController _storyAnimator;
    [SerializeField] private RuntimeAnimatorController _defaultAnimator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _uiObject;
    private ChaseGhost _chaseGhost;

    private bool isStoryEnded = false;

    public async UniTask ExecuteEvent(int index)
    {
        NpcAnimator.runtimeAnimatorController = _defaultAnimator;
        _chaseGhost.StartBehavior();
        _uiObject.SetActive(true);
    }

    public void FinishEvent(int index)
    {
        NpcAnimator.runtimeAnimatorController = _defaultAnimator;
        NpcAnimator.Rebind();
        NpcAnimator.Update(0f);

        if (_chaseGhost == null)
        {
            _chaseGhost = GetComponent<ChaseGhost>();
            _chaseGhost.EndBehavior();
        }

        _chaseGhost.StartBehavior();
        _uiObject.SetActive(true);

        isStoryEnded = true;
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
        if(_chaseGhost == null)
        {
            _chaseGhost = GetComponent<ChaseGhost>();
            _chaseGhost.EndBehavior();
        }

        if(isStoryEnded == false)
        {
            NpcAnimator.runtimeAnimatorController = _storyAnimator;
        }
    }
}
