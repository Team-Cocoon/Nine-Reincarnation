using UnityEngine;
using UnityEngine.Pool;

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

    private void InitPool()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject go = Instantiate(_spikePrefab);
                go.GetComponent<Spike>().SetPool(this);
                return go;
            },
            actionOnGet: (go) => go.SetActive(true),      
            actionOnRelease: (go) => go.SetActive(false), 
            actionOnDestroy: (go) => Destroy(go),        
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    public GameObject Get() => Pool.Get();
    public void Release(GameObject spike) => Pool.Release(spike);

    private void OnDisable() => _pool?.Clear();
}
