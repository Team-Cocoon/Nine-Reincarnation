using UnityEngine;
using VContainer;

public class LoadNextScene : MonoBehaviour
{
    [Inject] private SceneLoadManager _sceneLaodManger;
    [SerializeField] private string _playerTag = "Player";
    private bool _isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_playerTag)) return;

        if (_isTriggered) return; // 이미 실행됐으면 무시
        _isTriggered = true;
        gameObject.SetActive(false);
        _sceneLaodManger.LoadNextScene().Forget();
    }
}
