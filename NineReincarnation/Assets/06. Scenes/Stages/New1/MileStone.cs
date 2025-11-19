using UnityEngine;
using UnityEngine.UIElements;

public class MileStone : MonoBehaviour
{
    [SerializeField] private Color32 color;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private void OnValidate()
    {
        _spriteRenderer.color = color;
    }

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        SetColor(color);
    }

    public void SetColor(Color32 newColor)
    {
        _renderer.GetPropertyBlock(_propBlock);

        _propBlock.SetColor("_Color", newColor);

        _renderer.SetPropertyBlock(_propBlock);
    }
}
