using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventCamera : MonoBehaviour
{
    [SerializeField] private int _priority;
    [SerializeField] private int _endPriority;
    [SerializeField] private float _defaultOthoSize;
    [SerializeField] private CinemachineZoom _zoom;
    [SerializeField] private CinemachineShake _shake;

    private void Start()
    {
        Zoom().Forget();
    }

    public async UniTaskVoid Zoom()
    {
        await _zoom.Zoom(2.0f, 4.0f);
    }
    //public void Shake()
    //{
    //    _shake.Shake();
    //}
}
