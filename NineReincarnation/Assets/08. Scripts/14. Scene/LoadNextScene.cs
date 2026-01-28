using UnityEngine;
using VContainer;

public interface ILoadNext
{
    public void NextScene();
}

public class LoadNextScene : MonoBehaviour
{
    [Inject]         private SceneTrigger  _sceneTrigger;
    [SerializeField] private SceneLoadType _type;
    [SerializeField] private bool          _loadSceneOnStart = false;
    [SerializeField] private string        _playerTag        = "Player";
    private bool _isTriggered = false;

    public void Start()
    {
        if (_loadSceneOnStart)
        {
            _sceneTrigger.LoadScene(_type);
        }
    }

    public void NextScene()
    {
        if (_isTriggered) return; // 이미 실행됐으면 무시
        _isTriggered = true;
        gameObject.SetActive(false);
        _sceneTrigger.LoadScene(_type);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_playerTag)) return;
        NextScene();
    }
}
