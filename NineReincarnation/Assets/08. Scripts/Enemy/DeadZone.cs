using Player.Controller;
using UnityEngine;

public class DeadZone : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        player = go.GetComponent<PlayerController>();
        player?.Respawn();
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
