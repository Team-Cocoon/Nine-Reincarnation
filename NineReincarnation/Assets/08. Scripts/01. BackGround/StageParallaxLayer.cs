using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class StageParallaxLayer : MonoBehaviour
{
    private static readonly int HorizontalTilingId = Shader.PropertyToID("_HorizontalTiling");
    private static readonly int HorizontalOffsetId = Shader.PropertyToID("_HorizontalOffset");

    [Header("Renderer")]
    public SpriteRenderer targetRenderer;

    [SortingLayerName]
    public string sortingLayerName = "Background";

    public int sortingOrder;

    [Header("Camera Parallax")]
    [Min(0f)] public float horizontalSpeed = 0.2f;
    public bool allowVerticalMovement;
    [Min(0f)] public float verticalSpeed = 0.1f;

    [Header("Horizontal Repeat")]
    public bool repeatHorizontally = true;
    [Min(1f)] public float horizontalTiling = 3f;
    [Range(0f, 1f)] public float horizontalOffset;

    [Header("Ambient Motion")]
    public bool ambientMotion;
    public Vector2 ambientAxis = Vector2.right;
    [Min(0f)] public float ambientAmplitude = 0.1f;
    [Min(0f)] public float ambientCyclesPerSecond = 0.05f;
    [Range(0f, 360f)] public float ambientPhaseDegrees;

    private MaterialPropertyBlock propertyBlock;
    private Vector3 initialWorldPosition;
    private Vector3 initialLocalPosition;
    private bool isCameraChild;
    private bool initialized;

    public void Initialize(Transform cameraTarget)
    {
        EnsureRenderer();

        if (targetRenderer == null || cameraTarget == null)
        {
            initialized = false;
            return;
        }

        initialWorldPosition = transform.position;
        initialLocalPosition = transform.localPosition;
        isCameraChild = transform.IsChildOf(cameraTarget);
        initialized = true;
        ApplyRenderingSettings();
    }

    public void UpdateLayer(Transform cameraTarget, Vector3 initialCameraPosition)
    {
        if (!initialized)
        {
            Initialize(cameraTarget);
        }

        if (!initialized || cameraTarget == null)
        {
            return;
        }

        Vector3 cameraDelta = cameraTarget.position - initialCameraPosition;
        Vector3 ambientOffset = GetAmbientOffset();

        if (isCameraChild)
        {
            Vector3 nextLocalPosition = initialLocalPosition;
            float movement = -cameraDelta.x * horizontalSpeed;

            if (repeatHorizontally)
            {
                float repeatWidth = GetRepeatWorldWidth();

                if (repeatWidth > Mathf.Epsilon)
                {
                    movement = Mathf.Repeat(
                        movement + repeatWidth * 0.5f,
                        repeatWidth) - repeatWidth * 0.5f;
                }
            }

            nextLocalPosition.x += movement;

            if (allowVerticalMovement)
            {
                nextLocalPosition.y -= cameraDelta.y * verticalSpeed;
            }

            transform.localPosition = nextLocalPosition + ambientOffset;
        }
        else
        {
            Vector3 nextWorldPosition = initialWorldPosition;
            nextWorldPosition.x += cameraDelta.x * (1f - horizontalSpeed);
            nextWorldPosition.y += cameraDelta.y *
                (allowVerticalMovement ? 1f - verticalSpeed : 1f);
            transform.position = nextWorldPosition + ambientOffset;
        }

        ApplyMaterialProperties();
    }

    public void ApplyRenderingSettings()
    {
        EnsureRenderer();

        horizontalSpeed = Mathf.Max(0f, horizontalSpeed);
        verticalSpeed = Mathf.Max(0f, verticalSpeed);
        horizontalTiling = Mathf.Max(1f, horizontalTiling);
        horizontalOffset = Mathf.Repeat(horizontalOffset, 1f);
        ambientAmplitude = Mathf.Max(0f, ambientAmplitude);
        ambientCyclesPerSecond = Mathf.Max(0f, ambientCyclesPerSecond);

        if (targetRenderer == null)
        {
            return;
        }

        if (IsValidSortingLayer(sortingLayerName))
        {
            targetRenderer.sortingLayerName = sortingLayerName;
        }

        targetRenderer.sortingOrder = sortingOrder;
        ApplyMaterialProperties();
    }

    private void Reset()
    {
        EnsureRenderer();
        ApplyRenderingSettings();
    }

    private void OnValidate()
    {
        ApplyRenderingSettings();
    }

    private Vector3 GetAmbientOffset()
    {
        if (!ambientMotion || ambientAmplitude <= 0f || ambientAxis.sqrMagnitude <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        float phase = ambientPhaseDegrees * Mathf.Deg2Rad;
        float angle = Time.time * ambientCyclesPerSecond * Mathf.PI * 2f + phase;
        Vector2 direction = ambientAxis.normalized;
        return (Vector3)(direction * (Mathf.Sin(angle) * ambientAmplitude));
    }

    private float GetRepeatWorldWidth()
    {
        if (targetRenderer == null || targetRenderer.sprite == null)
        {
            return 0f;
        }

        return targetRenderer.sprite.bounds.size.x * Mathf.Abs(transform.lossyScale.x);
    }

    private void ApplyMaterialProperties()
    {
        if (targetRenderer == null)
        {
            return;
        }

        propertyBlock ??= new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(HorizontalTilingId, repeatHorizontally ? horizontalTiling : 1f);
        propertyBlock.SetFloat(HorizontalOffsetId, horizontalOffset);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void EnsureRenderer()
    {
        targetRenderer ??= GetComponent<SpriteRenderer>();
    }

    private static bool IsValidSortingLayer(string layerName)
    {
        foreach (SortingLayer layer in SortingLayer.layers)
        {
            if (layer.name == layerName)
            {
                return true;
            }
        }

        return false;
    }
}
