using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioManager.Bgm _bgm;

    void Start()
    {
        AudioManager.Instance.PlayBgm(_bgm);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopBgm();
    }
}
