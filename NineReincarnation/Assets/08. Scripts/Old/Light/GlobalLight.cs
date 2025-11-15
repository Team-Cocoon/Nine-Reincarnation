using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    private Light2D _light;
    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Start()
    {
        LightManager.Instance.OnVisionLimited += OffLight;
        LightManager.Instance.OnVisionLimitCleared += OnLight;
    }

    private void OnDestroy()
    {
        LightManager.Instance.OnVisionLimited -= OffLight;
        LightManager.Instance.OnVisionLimitCleared -= OnLight;
    }

    private void OnLight()
    {
        float intensity = 0.0f;
        DOTween.To(() => intensity, i =>
        {
            intensity = i;
            _light.intensity = intensity;
        }, 1.0f, 1f);
    }

    private void OffLight()
    {
        float intensity = 1.0f;
        DOTween.To(() => intensity, i =>
        {
            intensity = i;
            _light.intensity = intensity;
        }, 0.0f, 1f);
    }
}
