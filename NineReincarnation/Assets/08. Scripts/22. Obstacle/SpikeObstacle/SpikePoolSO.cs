using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SpikePoolSO", menuName = "Scriptable Objects/SpikePoolSO")]
public class SpikePoolSO : ScriptableObject
{
    [SerializeField] private GameObject _spikePrefab;
    private IObjectPool<GameObject> _pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (_pool == null) InitPool();
            return _pool;
        }
    }

    private void OnEnable()
    {
        // 씬이 언로드될 때 풀을 비우도록 이벤트 등록 (SO 생명주기 문제 해결)
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        _pool?.Clear();
        _pool = null;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // 씬이 바뀔 때 기존 하이라키의 객체들은 파괴되므로 풀 참조도 날려줍니다.
        _pool?.Clear();
        _pool = null;
    }

    private void InitPool()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject go = Instantiate(_spikePrefab);
                go.GetComponent<Spike>().SetPool(this);
                return go;
            },
            // [수정됨] 여기서 SetActive(true)를 하지 않습니다. 위치를 먼저 잡고 켜야 합니다.
            actionOnGet: (go) => { },
            actionOnRelease: (go) => go.SetActive(false),
            actionOnDestroy: (go) => Destroy(go),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    public GameObject Get() => Pool.Get();
    public void Release(GameObject spike) => Pool.Release(spike);
}