using System;
using DG.Tweening;
using UnityEngine;

public enum StatusEffect
{
    None,
    VisionLimited
}

public class LightManager : MonoBehaviour
{
    public static LightManager Instance { get; private set; }

    [SerializeField] private float _visionLimitedDuration = 6.0f;
    [SerializeField] private GameObject _limitedLight;
    private StatusEffect _state = StatusEffect.None;

    public Action OnVisionLimited;
    public Action OnVisionLimitCleared;

    public StatusEffect State
    {
        get => _state;
        set => _state = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Instantiate(_limitedLight);
        OnVisionLimited += OffLight;
    }

    private void OnDestroy()
    {
        OnVisionLimited -= OffLight;
    }

    private void OffLight()
    {
        DOVirtual.DelayedCall(_visionLimitedDuration, () =>
        {
            OnVisionLimitCleared();
        });
    }
}
