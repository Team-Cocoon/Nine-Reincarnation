using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform t;
    private void Awake()
    {
        Instance = this;
        _camera = GetComponent<CinemachineCamera>();
    }

    /// <summary>
    /// 카메라가 따라다니는 타겟 변경
    /// </summary>
    /// <param name="transform"></param>
    public void ChangeTarget(Transform transform)
    {
        _camera.Follow = transform;
    }
}
