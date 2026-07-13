using System.Threading;
using Cysharp.Threading.Tasks;
using Player.Controller;
using VContainer;

public class StageSubInitiator : IInitiator
{
    [Inject] private PlayerController _playerController;
    [Inject] private VirtualCameraManager _vCammanager;
    [Inject] private CheckPoint _checkPoint;

    public UniTask GameInitialize(CancellationToken token)
    {
        //플레이어 위치 가장 처음 체크포인트로
        _playerController.transform.position = _checkPoint.transform.position;
        _vCammanager.Initialize();

        return UniTask.CompletedTask;
    }
}
