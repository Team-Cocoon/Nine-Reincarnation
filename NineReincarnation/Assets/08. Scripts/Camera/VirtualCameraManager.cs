using Player.Controller;
using Unity.Cinemachine;
using Unity.ProjectAuditor.Editor;
using UnityEngine;
using VContainer;

enum CameraPriority
{
    None = 0,
    Priority = 1
}

public class VirtualCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _cams;
    [SerializeField] private PolygonCollider2D[] _areas;
    [SerializeField] private int _currentIndex = 0;
    [Inject] private Transform _player;

    private void Awake()
    {

        for(int i = 0; i < _areas.Length; ++i)
        {
            _areas[i].GetComponent<VCamArea>().Index = i;
        }

        for (int i = 0; i < _cams.Length; ++i)
        {
            _cams[i].GetComponent<CinemachineConfiner2D>().BoundingShape2D = _areas[i];
            _cams[i].GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
            _cams[i].Priority = (int)CameraPriority.None;
            _cams[i].Follow = _player;
        }


        _cams[0].Priority = (int)CameraPriority.Priority;
    }

    public void SetPrioriy(int index)
    {
        if (_currentIndex == index) return;

        _cams[_currentIndex].Priority = (int)CameraPriority.None;
        _currentIndex = index;
        _cams[_currentIndex].Priority = (int)CameraPriority.Priority;
    }
}
