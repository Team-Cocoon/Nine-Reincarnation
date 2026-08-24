using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryScroll : StoryNPC, IEventInterface, IClickInteractableToggle
{
    [SerializeField] private ChaseGhostUI _storyUI;
    [SerializeField] private Interaction _Interaction;
    [SerializeField] private AudioSource _audioSource;
    private bool isClick = false;

    public bool IsClickControlToSelf => false;

    public void DisableClickInteraction()
    {

    }

    public void EnableClickInteraction()
    {
        OpenStoryUI().Forget();
    }

    private async UniTaskVoid OpenStoryUI()
    {
        await _storyUI.OpenUI(this.GetCancellationTokenOnDestroy());
        isClick = true;
    }
    public void SoundPlay()
    {
        _audioSource.Play();
    }

    public async UniTask ExecuteEvent(int index)
    {
        switch (index)
        {
            case 0:
                _Interaction.IsInteraction = true;
                break;
            case 1:
                await UniTask.WaitUntil(() => isClick == true, cancellationToken: this.destroyCancellationToken);
                break;
            case 2:
                _Interaction.IsInteraction = false;
                break;
            default:
                return;
        }
    }

    public void FinishEvent(int index)
    {
        switch (index)
        {
            case 0:
                _Interaction.IsInteraction = true;
                break;
            case 1:
                break;
            case 2:
                _Interaction.IsInteraction = false;
                break;
            default:
                return;
        }
    }
}
