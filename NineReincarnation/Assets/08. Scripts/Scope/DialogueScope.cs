using UnityEngine;
using VContainer;
using VContainer.Unity;
public class DialogueScope : LifetimeScope
{
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private StoryAnimationManager _storyAnimationManager;
    [SerializeField] private StoryEventManager _storyEventManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<StoryAnimationManager>(_storyAnimationManager);
        builder.RegisterComponent<StoryEventManager>(_storyEventManager);
        builder.RegisterComponent<DialogueSpace.DialogueManager>(_dialogueManager);
    }

}
