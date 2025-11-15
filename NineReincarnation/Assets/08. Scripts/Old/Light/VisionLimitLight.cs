using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VisionLimitLight : MonoBehaviour
{
    [SerializeField] float _speed;
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();

        LightManager.Instance.OnVisionLimited += OnLight;
        LightManager.Instance.OnVisionLimitCleared += OffLight;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.parent = InputManager.Instance.CurPlayer.transform;
        transform.localPosition = Vector3.zero;
    }

    private void OnDestroy()
    {
        LightManager.Instance.OnVisionLimited -= OnLight;
        LightManager.Instance.OnVisionLimitCleared -= OffLight;
    }

    private void Update()
    {
        UpdateLightScale();
    }

    private void OnLight()
    {
        gameObject.SetActive(true);
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
        }, 0.0f, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            LightManager.Instance.State = StatusEffect.None;
        });
    }

    private void UpdateLightScale()
    {
        transform.localScale = Mathf.Sin(Time.time * _speed) * 0.02f * Vector3.one + Vector3.one;
    }
}
