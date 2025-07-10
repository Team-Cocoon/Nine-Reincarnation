using DG.Tweening;
using Player.Controller;
using UnityEngine;


namespace Map.Platform
{
    public class OneWayPlatform : MonoBehaviour, ICollidable
    {
        private Collider2D _platformCollider;
        private PlayerController player;
        private void Awake()
        {
            _platformCollider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// 대상과의 충돌을 무시
        /// </summary>
        /// <param name="value"></param>
        public void Ignore(Collider2D collider)
        {
            Physics2D.IgnoreCollision(_platformCollider, collider);
            ResetPlatform(collider);
        }

        public void Enter(GameObject go)
        {
            player = go.GetComponent<PlayerController>();
            player?.SetContactPlatform(this);
        }

        public void Exit(GameObject go)
        {
            return;
        }

        private void ResetPlatform(Collider2D collider)
        {
            player?.SetContactPlatform(); //접촉한 플랫폼 null로 초기화
            DOVirtual.DelayedCall(0.5f, () =>
            {
                Physics2D.IgnoreCollision(_platformCollider, collider, false);
            });
        }
    }

}
