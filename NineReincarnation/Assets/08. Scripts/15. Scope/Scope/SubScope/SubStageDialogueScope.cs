using UnityEngine;
using VContainer;

public class SubStageDialogueScope : SubStageScope
{

    [Header("----- StoryInstaller ------")]
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private StoryEventManager _storyEventManager;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        if ((_dialogueManager != null) || (_storyEventManager != null))
        {
            new StoryInstaller(_dialogueManager, _storyEventManager).Install(builder);
        }
    }
}
