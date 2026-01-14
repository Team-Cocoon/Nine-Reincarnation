using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StoryInstaller : IInstaller
{
    private readonly DialogueSpace.DialogueManager _dialogueManager;
    private readonly StoryEventManager _storyEventManager;
    //버블 매니저도 추가예정

    public StoryInstaller(
        DialogueSpace.DialogueManager dialogueManaer,
        StoryEventManager storyEventManager)
    {
        _dialogueManager = dialogueManaer;
        _storyEventManager = storyEventManager;
    }

    public void Install(IContainerBuilder builder)
    {
        StoryNPC[] allNpcs = Object.FindObjectsByType<StoryNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        BubbleUI[] allBubbles = Object.FindObjectsByType<BubbleUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        SelectUI[] allSelect = Object.FindObjectsByType<SelectUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        builder.RegisterEntryPoint<StoryAnimationManager>(Lifetime.Scoped)
            .WithParameter(allNpcs)
            .AsSelf();
        builder.RegisterEntryPoint<BubbleManager>(Lifetime.Scoped)
            .WithParameter(allBubbles)
            .AsSelf();
        builder.RegisterEntryPoint<SelectManager>(Lifetime.Scoped)
            .WithParameter(allSelect)
            .AsSelf();

        builder.RegisterComponent<StoryEventManager>(_storyEventManager);
        builder.RegisterComponent<DialogueSpace.DialogueManager>(_dialogueManager);
    }
}
