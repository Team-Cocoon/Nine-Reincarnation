using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    PlayerController player;

    public void Enter(GameObject go = null)
    {
        Debug.Log("체크포인트");
        player = go.GetComponent<PlayerController>();
        player?.SetCheckPoint(transform.position);
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
