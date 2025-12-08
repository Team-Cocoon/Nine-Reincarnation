using Player.Controller;
using UnityEngine;
using UnityEngine.LightTransport;
using VContainer;
using VContainer.Unity;

public class SubStageSceneScope : LifetimeScope
{
    [Header("----- StoryInstaller ------")]
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private StoryEventManager             _storyEventManager;

    [Header("----- SubSceneInstaller ------")]
    [SerializeField] private LoadNextScene _loadNextScene;

    [Header("----- VirtualCameraInstaller ------")]
    [SerializeField] private VirtualCameraManager _vCammanager;

    protected override void Configure(IContainerBuilder builder)
    {
        if((_dialogueManager != null) || (_storyEventManager != null))
        {
            new StoryInstaller(_dialogueManager, _storyEventManager).Install(builder);
        }
        new VirtualCameraInstaller(_vCammanager).Install(builder);

        builder.RegisterComponent<LoadNextScene>(_loadNextScene);

        PlayerController player = Parent.Container.Resolve<PlayerController>();
        player.ResetVelocityY();
    }
}
