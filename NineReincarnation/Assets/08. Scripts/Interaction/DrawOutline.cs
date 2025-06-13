using UnityEngine;

public class DrawOutline : MonoBehaviour, IInteractableToggle
{
    [Header("--- 아웃라인 설정 관련 변수 ---")]
    [SerializeField] private bool _isOutline;
    [ColorUsage(true, true)] //HDR 사용 여부
    [SerializeField] private Color _outlineColor;
    [SerializeField] private float _thickness;

    private Texture _tex;
    private Renderer _render;
    private MaterialPropertyBlock _propBlock;

    public bool IsOutline
    {
        get
        {
            return _isOutline;
        }
        set
        {
            _isOutline = value;
            SetOutline();
        }
    }

    public Color OutlineColor
    {
        get
        {
            return _outlineColor;
        }
        set
        {
            _outlineColor = value;
            SetOutline();
        }
    }

    public float Thickness
    {
        get
        {
            return _thickness;
        }
        set
        {
            _thickness = value;
            SetOutline();
        }
    }

    private void Awake()
    {
        _render = GetComponent<Renderer>();
        _tex = GetComponent<SpriteRenderer>().sprite.texture;
        _propBlock = new MaterialPropertyBlock();

        SetTexelSize();
        SetOutline();
    }

    private void SetTexelSize()
    {
        _render.GetPropertyBlock(_propBlock);
        _propBlock.SetVector("_OutlineTexelSize", new Vector2(_tex.width, _tex.height));
        _render.SetPropertyBlock(_propBlock);
    }

    public void SetOutline()
    {
        _render.GetPropertyBlock(_propBlock);
        _propBlock.SetInt("_IsOutline", _isOutline == true ? 1 : 0);
        _propBlock.SetColor("_OutlineColor", _outlineColor);
        _propBlock.SetFloat("_OutlineThickness", _thickness);
        _render.SetPropertyBlock(_propBlock);
    }

    public void EnableInteraction()
    {
        IsOutline = true;
    }

    public void DisableInteraction()
    {
        IsOutline = false;
    }
}
