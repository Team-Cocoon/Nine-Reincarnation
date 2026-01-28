using UnityEngine;
using UnityEngine.Playables;

public class StoryBird : MonoBehaviour
{
    [SerializeField] private PlayableDirector _timeline;
    [SerializeField] private GameObject _key;
    private bool _detectedPlayer = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _detectedPlayer)
        {
            _detectedPlayer = false;
            _key.SetActive(false);
            InputManager.Instance?.CurPlayer.gameObject.SetActive(false);
            _timeline.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _detectedPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _detectedPlayer = false;
        }
    }
}
