using Player.Controller;
using System;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

enum CameraPriority
{
    None = 0,
    Priority = 1
}

public class VirtualCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _eventCam;
    [SerializeField] private Transform _storyObj;

    [SerializeField] private CinemachineCamera[] _cams;
    [SerializeField] private PolygonCollider2D[] _areas;
    [SerializeField] private int _currentIndex = 0;
    private int _prevIndex = 0;
    private bool _isEventCamFocused = false;
    [Inject] private PlayerController _player;

    [SerializeField] private DialogueCameraShiftHelper _cameraShiftHelper;

    public void Initialize()
    {
        for (int i = 0; i < _areas.Length; ++i)
        {
            _areas[i].GetComponent<VCamArea>().Index = i;
        }

        for (int i = 0; i < _cams.Length; ++i)
        {
            _cams[i].GetComponent<CinemachineConfiner2D>().BoundingShape2D = _areas[i];
            _cams[i].GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
            _cams[i].Priority = (int)CameraPriority.None;
            _cams[i].Follow = _player.transform;
        }


        if (_eventCam != null)
        {
            _eventCam.Follow = _storyObj;
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

    public void SetPlayer()
    {
        _eventCam.Follow = _player.transform;
    }

    public void SetFollowObj(Transform objTransfrom)
    {
        _eventCam.Follow = objTransfrom;
    }

    public void SetFollowObj(string name)
    {
        SetFollowObj(_cameraShiftHelper.GetShiftObjTransform(name));
    }

    public void SetToEventCam()
    {
        if (_isEventCamFocused == true)
            return;

        _isEventCamFocused = true;
        _prevIndex = _currentIndex;

        _cams[_currentIndex].Priority = (int)CameraPriority.None;
        _eventCam.Priority = (int)CameraPriority.Priority;
    }

    public void ResetToNormalCam()
    {
        if (_isEventCamFocused == false)
            return;

        _isEventCamFocused = false;

        _cams[_currentIndex].Priority = (int)CameraPriority.Priority;
        _eventCam.Priority = (int)CameraPriority.None;
    }
}
