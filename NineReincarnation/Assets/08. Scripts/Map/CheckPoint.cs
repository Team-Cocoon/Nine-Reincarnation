using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    private PlayerController player;

    public void Enter(GameObject go = null)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
        player = go.GetComponent<PlayerController>();
        player?.SetCheckPoint(transform.position);
        if (player != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
