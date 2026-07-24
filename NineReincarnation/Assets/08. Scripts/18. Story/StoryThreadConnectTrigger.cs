using DialogueSpace;
using UnityEngine;
using UnityEngine.Rendering;

public class StoryThreadConnectTrigger : MonoBehaviour
{
    // 무조건 플레이어와 threadHolder를 연결한다고 가정

    [SerializeField] private StoryThread _storyThread;
    [SerializeField] private float _threadFadeTime = 0.5f;
    [SerializeField] private Transform _threadHolder;
    [SerializeField] private string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_playerTag)) return;

        _storyThread.Connect(collision.gameObject.transform, _threadHolder, 0.5f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(_playerTag)) return;

        _storyThread.Disconnect();
    }
}
