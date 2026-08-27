using Cysharp.Threading.Tasks;
using Player.Controller;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class StageCheatSetter : MonoBehaviour
{
    [Inject] private PlayerController _anna;
    [Inject] private CheatManager _cheatManager;

    [SerializeField] private List<Transform> _stageMovePoints;

    private void Awake()
    {
        _cheatManager.OnCheatMoveEnd.RemoveAllListeners();

        CheckFirstCheat().Forget();
    }

    private async UniTaskVoid CheckFirstCheat()
    {
        await UniTask.Yield();

        if (_cheatManager.IsMapMovedByCheat() == false)
        {
            _cheatManager.OnCheatMoveEnd.AddListener(MoveToPoint);
            return;
        }

        CheatInfo info = _cheatManager.GetCheatInfo;
        if (info == null)
            return;

        MoveToPoint(info);

        _cheatManager.OnCheatMoveEnd.AddListener(MoveToPoint);
    }

    private void MoveToPoint(CheatInfo info)
    {
        _anna.gameObject.SetActive(true);
        _anna.transform.position = _stageMovePoints[info.PointIndex].position;

        _anna?.SetCheckPoint(transform.position);

        _cheatManager.DoneCheatMoving();
    }
}
