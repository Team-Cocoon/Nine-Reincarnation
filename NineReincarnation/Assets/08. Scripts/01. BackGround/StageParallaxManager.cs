using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SortingLayerNameAttribute : PropertyAttribute
{
}

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public sealed class StageParallaxManager : MonoBehaviour
{
    [Serializable]
    private sealed class Layer
    {
        [Tooltip("관리할 배경 SpriteRenderer입니다.")]
        public SpriteRenderer renderer;

        [Tooltip("카메라가 가로로 움직일 때 이미지가 화면에서 흐르는 속도입니다.")]
        [Min(0f)] public float horizontalSpeed = 0.2f;

        [Tooltip("카메라의 세로 이동에 이 레이어가 반응할지 결정합니다.")]
        public bool allowVerticalMovement;

        [Tooltip("카메라가 세로로 움직일 때 이미지가 화면에서 흐르는 속도입니다.")]
        [Min(0f)] public float verticalSpeed = 0.1f;

        [Tooltip("이미지를 가로로 몇 번 반복해서 표시할지 정합니다. 세로는 반복하지 않습니다.")]
        public bool repeatHorizontally = true;

        [Min(1f)] public float horizontalTiling = 1f;

        [Range(0f, 1f)] public float horizontalOffset;

        [SortingLayerName]
        public string sortingLayerName = "Default";

        public int sortingOrder;

        [NonSerialized] public Vector3 initialWorldPosition;
        [NonSerialized] public Vector3 initialLocalPosition;
        [NonSerialized] public bool isCameraChild;
    }

    private static readonly int HorizontalTilingId = Shader.PropertyToID("_HorizontalTiling");
    private static readonly int HorizontalOffsetId = Shader.PropertyToID("_HorizontalOffset");

    [Header("Target")]
    [Tooltip("비어 있으면 MainCamera를 자동으로 사용합니다.")]
    [SerializeField] private Transform cameraTarget;

    [Header("Layers")]
    [Tooltip("배경 레이어를 뒤에서 앞으로 등록합니다.")]
    [SerializeField] private List<Layer> layers = new();

    private MaterialPropertyBlock propertyBlock;
    private Vector3 initialCameraPosition;
    private bool initialized;

    private void OnEnable()
    {
        initialized = false;
        ApplyRenderingSettings();
        TryInitialize();
    }

    private void LateUpdate()
    {
        if (!TryInitialize())
        {
            return;
        }

        Vector3 cameraDelta = cameraTarget.position - initialCameraPosition;

        foreach (Layer layer in layers)
        {
            UpdateLayer(layer, cameraDelta);
        }
    }

    private void OnValidate()
    {
        ApplyRenderingSettings();
    }

    private bool TryInitialize()
    {
        if (initialized && cameraTarget != null)
        {
            return true;
        }

        if (cameraTarget == null && Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
        }

        if (cameraTarget == null)
        {
            return false;
        }

        initialCameraPosition = cameraTarget.position;

        foreach (Layer layer in layers)
        {
            if (layer.renderer != null)
            {
                layer.initialWorldPosition = layer.renderer.transform.position;
                layer.initialLocalPosition = layer.renderer.transform.localPosition;
                layer.isCameraChild = layer.renderer.transform.IsChildOf(cameraTarget);
            }
        }

        initialized = true;
        return true;
    }

    private void UpdateLayer(Layer layer, Vector3 cameraDelta)
    {
        if (layer.renderer == null)
        {
            return;
        }

        EnsurePropertyBlock();

        Transform layerTransform = layer.renderer.transform;

        if (layer.isCameraChild)
        {
            Vector3 nextLocalPosition = layer.initialLocalPosition;
            float unwrappedMovement = -cameraDelta.x * layer.horizontalSpeed;

            if (layer.repeatHorizontally)
            {
                float repeatWidth = GetRepeatWorldWidth(layer);

                if (repeatWidth > Mathf.Epsilon)
                {
                    float wrappedMovement = Mathf.Repeat(unwrappedMovement + repeatWidth * 0.5f, repeatWidth)
                        - repeatWidth * 0.5f;
                    nextLocalPosition.x += wrappedMovement;
                }
            }
            else
            {
                nextLocalPosition.x += unwrappedMovement;
            }

            if (layer.allowVerticalMovement)
            {
                nextLocalPosition.y -= cameraDelta.y * layer.verticalSpeed;
            }

            layerTransform.localPosition = nextLocalPosition;
        }
        else
        {
            Vector3 nextPosition = layer.initialWorldPosition;

            // Fallback for layers that are not parented below the camera.
            nextPosition.x += cameraDelta.x * (1f - layer.horizontalSpeed);
            nextPosition.y += cameraDelta.y * (layer.allowVerticalMovement ? 1f - layer.verticalSpeed : 1f);
            layerTransform.position = nextPosition;
        }

        layer.renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(HorizontalTilingId, GetEffectiveTiling(layer));
        propertyBlock.SetFloat(HorizontalOffsetId, layer.horizontalOffset);
        layer.renderer.SetPropertyBlock(propertyBlock);
    }

    private void ApplyRenderingSettings()
    {
        EnsurePropertyBlock();

        foreach (Layer layer in layers)
        {
            layer.horizontalSpeed = Mathf.Max(0f, layer.horizontalSpeed);
            layer.verticalSpeed = Mathf.Max(0f, layer.verticalSpeed);
            layer.horizontalTiling = Mathf.Max(1f, layer.horizontalTiling);
            layer.horizontalOffset = Mathf.Repeat(layer.horizontalOffset, 1f);

            if (layer.renderer == null)
            {
                continue;
            }

            if (IsValidSortingLayer(layer.sortingLayerName))
            {
                layer.renderer.sortingLayerName = layer.sortingLayerName;
            }

            layer.renderer.sortingOrder = layer.sortingOrder;
            layer.renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(HorizontalTilingId, GetEffectiveTiling(layer));
            propertyBlock.SetFloat(HorizontalOffsetId, layer.horizontalOffset);
            layer.renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private static float GetRepeatWorldWidth(Layer layer)
    {
        if (layer.renderer.sprite == null)
        {
            return 0f;
        }

        float spriteWorldWidth =
            layer.renderer.sprite.bounds.size.x *
            Mathf.Abs(layer.renderer.transform.lossyScale.x);

        return spriteWorldWidth;
    }

    private static float GetEffectiveTiling(Layer layer)
    {
        return layer.repeatHorizontally ? layer.horizontalTiling : 1f;
    }

    private static bool IsValidSortingLayer(string sortingLayerName)
    {
        foreach (SortingLayer sortingLayer in SortingLayer.layers)
        {
            if (sortingLayer.name == sortingLayerName)
            {
                return true;
            }
        }

        return false;
    }

    private void EnsurePropertyBlock()
    {
        propertyBlock ??= new MaterialPropertyBlock();
    }
}
