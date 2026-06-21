using UnityEngine;

public class MouseTest : MonoBehaviour
{
    [SerializeField] private KeyCode _leftClickKeyCode;
    [SerializeField] private KeyCode _rightClickKeyCode;

    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _leftClickSprite;
    [SerializeField] private Sprite _rightClickSprite;
    [SerializeField] private SpriteRenderer _renderer;

    private void Update()
    {
        if (Input.GetKeyDown(_leftClickKeyCode))
        {
            _renderer.sprite = _leftClickSprite;
        }
        else if (Input.GetKeyUp(_leftClickKeyCode))
        {
            _renderer.sprite = _defaultSprite;
        }

        if (Input.GetKeyDown(_rightClickKeyCode))
        {
            _renderer.sprite = _rightClickSprite;
        }
        else if (Input.GetKeyUp(_rightClickKeyCode))
        {
            _renderer.sprite = _defaultSprite;
        }
    }
}
