using TMPro;
using UnityEngine;

public class KeyboardTest : MonoBehaviour
{
    [SerializeField] private KeyCode _keyCode;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Color32 _downColor;
    [SerializeField] private SpriteRenderer _sprite;

    private Color32 _defaultColor;

    private void OnValidate()
    {
        _text.text = _keyCode.ToString();
    }

    private void Awake()
    {
        _defaultColor = _sprite.color;
    }

    private void Update()
    {
        if(Input.GetKeyDown(_keyCode))
        {
            _sprite.color = _downColor;
            transform.localPosition = transform.localPosition + Vector3.down * 0.1f;
        }
        else if(Input.GetKeyUp(_keyCode))
        {
            _sprite.color = _defaultColor;
            transform.localPosition = transform.localPosition - Vector3.down * 0.1f;
        }
    }
}
