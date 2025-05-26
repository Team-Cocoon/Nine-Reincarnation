using Player.Controller;
using UnityEngine;

public class WickedGhost : MonoBehaviour, ICollidable
{
    public void Enter(GameObject go = null)
    {
        if(LightManager.Instance.State != StatusEffect.VisionLimited)
        {
            LightManager.Instance.State = StatusEffect.VisionLimited;
            LightManager.Instance.OnVisionLimited();
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
