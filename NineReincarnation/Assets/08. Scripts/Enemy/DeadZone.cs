using Player.Controller;
using UnityEngine;

public class DeadZone : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        player = go.GetComponent<PlayerController>();
        player?.Dead();
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
