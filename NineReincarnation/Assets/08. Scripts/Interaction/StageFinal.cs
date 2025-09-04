using DG.Tweening;
using Player.Controller;
using UnityEngine;
using UnityEngine.Playables;

public class StageFinal : MonoBehaviour, ICollidable
{
    [SerializeField] private PlayableDirector _timeLine;
    [SerializeField] private GameObject _animationPlayer;

    private PlayerController _player;

    private Tween tween;

    public void Enter(GameObject go = null)
    {
        _player = go.GetComponent<PlayerController>();
        if (_player != null)
        {
            _player.gameObject.SetActive(false);
            _animationPlayer.transform.position = _player.transform.position;
            _timeLine.Play();
        }
    }

    public void DestroyThread()
    {
        Destroy(gameObject.transform.root.gameObject);
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
