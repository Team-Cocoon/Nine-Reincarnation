using Player.Controller;
using UnityEngine;

public class WickedGhost : MonoBehaviour, ICollidable
{
    [Header("---- 업화 관리 ----")]
    [SerializeField] private GameObject _hellFire;
    [SerializeField] private float _duration; //생성간격

    private float _coolTime = 0.0f;

    private void Update()
    {
        if(_coolTime <= float.Epsilon)
        {
            _coolTime = _duration;
            CreateHellFire();
        }
        _coolTime -= Time.deltaTime;
    }

    private void CreateHellFire()
    {
        Instantiate(_hellFire).transform.position = transform.position;
    }

    public void Enter(GameObject go = null)
    {
        if(LightManager.Instance.State != StatusEffect.VisionLimited)
        {
            LightManager.Instance.State = StatusEffect.VisionLimited;
            LightManager.Instance.OnVisionLimited();
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
