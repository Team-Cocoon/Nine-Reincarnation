using UnityEngine;


namespace Map.Platform
{
    public class OneWayPlatform : MonoBehaviour, ICollidable
    {
        // Marker component. PlayerController's raycast motor decides whether this
        // surface blocks movement; no Physics2D.IgnoreCollision timer is needed.
        public void Enter(GameObject go) { }
        public void Exit(GameObject go) { }
    }

}
