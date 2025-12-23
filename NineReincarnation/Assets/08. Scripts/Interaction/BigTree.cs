using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BigTree : DrawOutline, IClickInteractableToggle, IEventInterface
{
    [SerializeField] private Interaction _Interaction;
    [SerializeField] private AudioSource _audioSource;
    public bool IsClickControlToSelf => false;

    public event Action SetAction;

    private bool isClick = false;

    public void DisableClickInteraction()
    {
        return;
    }

    public void EnableClickInteraction()
    {
        isClick = true;
    }
    public void SoundPlay()
    {
        _audioSource.Play();
    }

    public async UniTask ExecuteEvent(int index)
    {
        switch(index)
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
}