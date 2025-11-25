using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MileStone : MonoBehaviour
{
    [SerializeField] private Color32 _color;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _panel;

    private MaterialPropertyBlock _propBlock;
    private Vector2               _defaultScale;
    private string                _palyerTag = "player";
    private Tween                 _tween;
    private Vector2               _targetScale = Vector2.zero;
    private void Awake()
    {
        _defaultScale = _panel.transform.localScale;

        _propBlock = new MaterialPropertyBlock();
        SetColor(_color);

        _panel.transform.localScale = _targetScale;
    }

    private void OnDisable()
    {
        _tween.Kill();
    }

    private void OpenPanel()
    {
        _panel.SetActive(true);

        _tween = _panel.transform.DOScale(_defaultScale, 0.5f).SetEase(Ease.OutBack);
    }

    private void ClosePanel()
    {
        _tween = _panel.transform.DOScale(_targetScale, 1f).SetEase(Ease.InBack);

        _panel.SetActive(false);
    }

    public void SetColor(Color32 newColor)
    {
        _renderer.GetPropertyBlock(_propBlock);

        _propBlock.SetColor("_Color", newColor);

        _renderer.SetPropertyBlock(_propBlock);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(_palyerTag))
        {
            OpenPanel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_palyerTag))
        {
            ClosePanel();
        }
    }
}
