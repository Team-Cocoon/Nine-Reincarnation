using DG.Tweening;
using EventHandler;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    private Camera _camera;

    [Header("--- 셰이크 ---")]
    public float _shakeDuration;
    public float _strength;
    public int _vibrato;
    public float _randomness;
    public bool _isFadeOut;

    [Header("--- 줌 ---")]
    public float _endValue;
    public float _duration;

    public bool isShake = false;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Start()
    {
        if(isShake)
        {
            CameraEventHandler.Shake(_camera, _duration, _strength, _vibrato, _randomness, _isFadeOut);
        }
        else
        {
            CameraEventHandler.Zoom(_camera, _endValue, _duration);
        }
    }
}
