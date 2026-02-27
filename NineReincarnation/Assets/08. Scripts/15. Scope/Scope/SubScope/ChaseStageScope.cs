using Player.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ChaseStageScope : LifetimeScope
{
    [Header("----- StoryInstaller ------")]
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private StoryEventManager _storyEventManager;

    [Header("----- SubSceneInstaller ------")]
    //[SerializeField] private LoadNextScene _loadNextScene;

    [Header("----- VirtualCameraInstaller ------")]
    [SerializeField] private VirtualCameraManager _vCammanager;

    [Header("----- CheckPoint ------")]
    [SerializeField] private Transform _checkPoint;

    [Header("----- Chase Scene -----")]
    [SerializeField] private ChaseGhost _chaseGhost;
    [SerializeField] private float _playerSpeed = 6.0f;
    [SerializeField] private float _defaultPlayerSpeed = 4.0f;

    private PlayerController player;
    protected override void Configure(IContainerBuilder builder)
    {
        if ((_dialogueManager != null) || (_storyEventManager != null))
        {
            new StoryInstaller(_dialogueManager, _storyEventManager).Install(builder);
        }
        new VirtualCameraInstaller(_vCammanager).Install(builder);

        //builder.RegisterComponent<LoadNextScene>(_loadNextScene);
        builder.RegisterComponent<ChaseGhost>(_chaseGhost);

        player = Parent.Container.Resolve<PlayerController>();
        player.ResetVelocityY();
    }

    private void Start()
    {
        player.transform.position = _checkPoint.position;
        player.Speed = _playerSpeed;
    }

    protected override void OnDestroy()
    {
        player.Speed = _defaultPlayerSpeed;
    }
}
