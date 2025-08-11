using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
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
