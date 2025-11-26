using UnityEngine;

public class MouseTest : MonoBehaviour
{
    [SerializeField] private KeyCode _keyCode;

    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _changeSprite;
    [SerializeField] private SpriteRenderer _renderer;

    private void Update()
    {
        if (Input.GetKeyDown(_keyCode))
        {
            _renderer.sprite = _changeSprite;
        }
        else if (Input.GetKeyUp(_keyCode))
        {
            _renderer.sprite = _defaultSprite;
        }
    }
}
