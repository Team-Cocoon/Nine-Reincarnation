using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities;

public class SubSceneLoader : SceneLoader
{
    [SerializeField] private string subScenePath;

    protected override void Awake()
    {
        base.Awake();
        
        _loadSceneCount++;
    }

    private async void Start()
    {
        await LoadSubScene();
        _loadSceneCount--;
    }

    public async UniTask LoadSubScene()
    {
        await LoadLoadingScene();

        int loadSceneCount = _loadSceneCount;
        await UnloadLastScene();
        await LoadSceneByPath(subScenePath);

        Debug.Log(string.Format("Sub {0}, {1}", loadSceneCount, _loadSceneCount));
        await UniTask.WaitUntil(() => loadSceneCount == _loadSceneCount, cancellationToken: _token);

        Debug.Log(string.Format("서브 종료"));
        await UnLoadLoadingScene();
    }
}
