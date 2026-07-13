using UnityEngine;

public class TriggerEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _object;
    [SerializeField] private string _tag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_tag) && !_object.activeSelf)
        {
            _object.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_tag) && _object.activeSelf)
        {
            _object.SetActive(false);
        }
    }
}
