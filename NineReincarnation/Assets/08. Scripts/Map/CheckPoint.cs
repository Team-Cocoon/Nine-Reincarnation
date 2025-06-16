using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.SavePoint);
        player = go.GetComponent<PlayerController>();
        player?.SetCheckPoint(transform.position);
        if (player != null)
        {
            Destroy(gameObject);
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
