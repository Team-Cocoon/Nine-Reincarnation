using UnityEngine;
using VContainer;

public class LoadNextScene : MonoBehaviour
{
    [Inject] private SceneLoadManager _sceneLaodManger;
    private bool _isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTriggered) return; // 이미 실행됐으면 무시
        _isTriggered = true;
        gameObject.SetActive(false);
        _sceneLaodManger.LoadNextScene().Forget();
    }
}
