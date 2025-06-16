using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        player = go.GetComponent<PlayerController>();
        player?.SetCheckPoint(transform.position);
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
