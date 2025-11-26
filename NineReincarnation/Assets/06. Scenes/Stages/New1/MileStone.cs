using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class MileStone : MonoBehaviour
{
    [SerializeField] private Color32 _color;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _panel;
    [SerializeField] private VideoPlayer _videoPlayer;

    private MaterialPropertyBlock _propBlock;
    private Vector2 _defaultScale;
    private string _palyerTag = "Player";
    private Tween _tween;
    private Vector2 _targetScale = Vector2.zero;

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
        _tween.Kill();

        _videoPlayer.Play();

        _tween = _panel.transform.DOScale(_defaultScale, 0.5f).SetEase(Ease.OutBack);
    }

    private void ClosePanel()
    {
        _tween.Kill();

        _tween = _panel.transform.DOScale(_targetScale, 0.5f).SetEase(Ease.InBack)
            .OnComplete(() => _videoPlayer.Pause());
    }

    public void SetColor(Color32 newColor)
    {
        _renderer.GetPropertyBlock(_propBlock);

        _propBlock.SetColor("_Color", newColor);

        _renderer.SetPropertyBlock(_propBlock);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_palyerTag))
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
