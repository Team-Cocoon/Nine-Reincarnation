using DG.Tweening;
using Player.Controller;
using UnityEngine;

public class StageFinal : MonoBehaviour, ICollidable
{
    private PlayerController _player;

    private Tween tween;
    public void Enter(GameObject go = null)
    {
        _player = go.GetComponent<PlayerController>();
        _player?.SetCheckPoint(transform.position);
        if (_player != null)
        {
            Destroy(gameObject.transform.root.gameObject);
            //_player.TutorialEnd();
            //tween = DOVirtual.DelayedCall(3f, () =>
            //{
            //    Destroy(gameObject.transform.root.gameObject);
            //});
        }
    }

    private void OnDestroy()
    {
        tween.Kill();
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
