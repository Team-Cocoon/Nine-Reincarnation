using Map.Platform;
using UnityEngine;

namespace Player.Controller
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public sealed class PlatformerRaycastMotor2D : MonoBehaviour
    {
        public struct CollisionInfo
        {
            public bool Above;
            public bool Below;
            public bool Left;
            public bool Right;
            public bool ClimbingSlope;
            public bool DescendingSlope;
            public float SlopeAngle;
            public Collider2D GroundCollider;

            public void Reset()
            {
                Above = Below = Left = Right = false;
                ClimbingSlope = DescendingSlope = false;
                SlopeAngle = 0f;
                GroundCollider = null;
            }
        }

        [SerializeField, Min(3)] private int _horizontalRayCount = 4;
        [SerializeField, Min(3)] private int _verticalRayCount = 4;
        [SerializeField, Min(0.001f)] private float _skinWidth = 0.02f;
        [SerializeField, Range(0f, 89f)] private float _maxSlopeAngle = 55f;

        private Collider2D _collider;
        private Rigidbody2D _body;
        private LayerMask _collisionMask;
        private int _slopeLayer;
        private RaycastOrigins _origins;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        private readonly RaycastHit2D[] _sweepHits = new RaycastHit2D[16];

        public CollisionInfo Collisions;

        private struct RaycastOrigins
        {
            public Vector2 TopLeft;
            public Vector2 TopRight;
            public Vector2 BottomLeft;
            public Vector2 BottomRight;
            public Vector2 SideBottomLeft;
            public Vector2 SideBottomRight;
            public Vector2 SideTopLeft;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _body = GetComponent<Rigidbody2D>();
            _slopeLayer = LayerMask.NameToLayer("Slope");
            // Several stage boundary walls (including Stage1-1 LeftWall/RightWall)
            // are still on Default, so Default must be part of the solid mask.
            _collisionMask = LayerMask.GetMask("Default", "Ground", "Obstacle", "Platform", "Slope");
        }

        public void Move(Vector2 displacement, bool dropThroughOneWay, bool allowSlopeMovement)
        {
            Collisions.Reset();
            UpdateRaycastOrigins();

            if (allowSlopeMovement && displacement.y < 0f && !dropThroughOneWay)
                DescendSlope(ref displacement);
            if (!Mathf.Approximately(displacement.x, 0f))
                HorizontalCollisions(ref displacement, allowSlopeMovement);
            if (!Mathf.Approximately(displacement.x, 0f)) SolidHorizontalSweep(ref displacement);
            if (!Mathf.Approximately(displacement.y, 0f)) VerticalCollisions(ref displacement, dropThroughOneWay);

            _body.position += displacement;
        }

        // Wall hang deliberately uses two points instead of the last horizontal
        // collision result. This prevents a single corner/ledge ray from being
        // mistaken for a full wall.
        public bool TryGetVerticalWall(int direction, out Collider2D wallCollider)
        {
            wallCollider = null;
            if (direction == 0) return false;

            UpdateRaycastOrigins();
            float height = _origins.SideTopLeft.y - _origins.SideBottomLeft.y;
            Vector2 side = direction < 0 ? _origins.SideBottomLeft : _origins.SideBottomRight;
            Vector2 rayDirection = Vector2.right * Mathf.Sign(direction);
            float rayDistance = _skinWidth * 3f + 0.02f;

            RaycastHit2D footHit = FindHit(side + Vector2.up * (height * 0.15f), rayDirection,
                rayDistance, false, false);
            RaycastHit2D chestHit = FindHit(side + Vector2.up * (height * 0.65f), rayDirection,
                rayDistance, false, false);

            if (!IsVerticalWallHit(footHit) || !IsVerticalWallHit(chestHit)) return false;

            wallCollider = footHit.collider;
            return true;
        }

        private bool IsVerticalWallHit(RaycastHit2D hit)
        {
            return hit.collider != null && !IsSlopeCollider(hit.collider) && Mathf.Abs(hit.normal.x) >= 0.9f;
        }

        private void HorizontalCollisions(ref Vector2 displacement, bool allowSlopeMovement)
        {
            float directionX = Mathf.Sign(displacement.x);
            float rayLength = Mathf.Abs(displacement.x) + _skinWidth;
            Vector2 origin = directionX < 0f ? _origins.SideBottomLeft : _origins.SideBottomRight;

            for (int i = 0; i < _horizontalRayCount; i++)
            {
                Vector2 rayOrigin = origin + Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = FindHit(rayOrigin, Vector2.right * directionX, rayLength, false, false);
#if UNITY_EDITOR
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,
                    hit.collider == null ? Color.yellow : Color.red);
#endif
                if (hit.collider == null) continue;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                bool isSlope = IsSlopeCollider(hit.collider);
                if (allowSlopeMovement && i == 0 && isSlope && slopeAngle > 0f && slopeAngle <= _maxSlopeAngle)
                {
                    if (Collisions.DescendingSlope)
                    {
                        Collisions.DescendingSlope = false;
                        displacement = new Vector2(displacement.x, 0f);
                    }

                    float distanceToSlope = 0f;
                    if (!Mathf.Approximately(slopeAngle, Collisions.SlopeAngle))
                    {
                        distanceToSlope = hit.distance - _skinWidth;
                        displacement.x -= distanceToSlope * directionX;
                    }

                    ClimbSlope(ref displacement, slopeAngle, hit.collider);
                    displacement.x += distanceToSlope * directionX;
                }

                if (Collisions.ClimbingSlope && isSlope && slopeAngle <= _maxSlopeAngle) continue;
                if (Mathf.Abs(hit.normal.x) < 0.1f) continue;

                displacement.x = Mathf.Max(0f, hit.distance - _skinWidth) * directionX;
                rayLength = hit.distance;
                Collisions.Left = directionX < 0f;
                Collisions.Right = directionX > 0f;
            }
        }

        private void SolidHorizontalSweep(ref Vector2 displacement)
        {
            float directionX = Mathf.Sign(displacement.x);
            float distance = Mathf.Abs(displacement.x) + _skinWidth;
            ContactFilter2D filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _collisionMask,
                useTriggers = false
            };

            int hitCount = _collider.Cast(Vector2.right * directionX, filter, _sweepHits, distance);
            float allowedDistance = Mathf.Abs(displacement.x);
            bool blocked = false;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _sweepHits[i];
                if (hit.collider == null || hit.collider.transform.IsChildOf(transform)) continue;
                if (hit.collider.GetComponentInParent<OneWayPlatform>() != null) continue;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (IsSlopeCollider(hit.collider) && slopeAngle > 0f && slopeAngle <= _maxSlopeAngle) continue;
                if (Mathf.Abs(hit.normal.x) < 0.1f) continue;

                allowedDistance = Mathf.Min(allowedDistance, Mathf.Max(0f, hit.distance - _skinWidth));
                blocked = true;
            }

            if (!blocked) return;
            displacement.x = allowedDistance * directionX;
            Collisions.Left = directionX < 0f;
            Collisions.Right = directionX > 0f;
        }

        private void VerticalCollisions(ref Vector2 displacement, bool dropThroughOneWay)
        {
            float directionY = Mathf.Sign(displacement.y);
            float rayLength = Mathf.Abs(displacement.y) + _skinWidth;
            Vector2 origin = directionY < 0f ? _origins.BottomLeft : _origins.TopLeft;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = origin + Vector2.right * (_verticalRaySpacing * i + displacement.x);
                RaycastHit2D hit = FindHit(rayOrigin, Vector2.up * directionY, rayLength,
                    directionY < 0f, dropThroughOneWay);
                if (hit.collider == null) continue;

                // A ray that starts inside an adjacent wall reports distance 0.
                // Never turn that overlap into movement in the opposite direction
                // (a downward ray previously produced an upward skin-width step).
                displacement.y = Mathf.Max(0f, hit.distance - _skinWidth) * directionY;
                rayLength = hit.distance;
                Collisions.Below = directionY < 0f;
                Collisions.Above = directionY > 0f;
                if (directionY < 0f) Collisions.GroundCollider = hit.collider;

                if (Collisions.ClimbingSlope)
                {
                    displacement.x = displacement.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) *
                                     Mathf.Sign(displacement.x);
                }
            }
        }

        private void ClimbSlope(ref Vector2 displacement, float slopeAngle, Collider2D ground)
        {
            float moveDistance = Mathf.Abs(displacement.x);
            float climbY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            if (displacement.y > climbY) return;

            displacement.y = climbY;
            displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(displacement.x);
            Collisions.Below = true;
            Collisions.ClimbingSlope = true;
            Collisions.SlopeAngle = slopeAngle;
            Collisions.GroundCollider = ground;
        }

        private void DescendSlope(ref Vector2 displacement)
        {
            float directionX = Mathf.Sign(displacement.x);
            Vector2 rayOrigin = directionX < 0f ? _origins.BottomRight : _origins.BottomLeft;
            RaycastHit2D hit = FindHit(rayOrigin, Vector2.down, Mathf.Infinity, true, false);
            if (hit.collider == null) return;
            if (!IsSlopeCollider(hit.collider)) return;

            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle <= 0f || slopeAngle > _maxSlopeAngle || Mathf.Sign(hit.normal.x) != directionX) return;
            if (hit.distance - _skinWidth > Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(displacement.x)) return;

            float moveDistance = Mathf.Abs(displacement.x);
            displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * directionX;
            displacement.y -= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            Collisions.SlopeAngle = slopeAngle;
            Collisions.DescendingSlope = true;
            Collisions.Below = true;
            Collisions.GroundCollider = hit.collider;
        }

        private RaycastHit2D FindHit(Vector2 origin, Vector2 direction, float distance, bool allowOneWay, bool dropThrough)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, _collisionMask);
            RaycastHit2D best = default;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null || hit.collider.isTrigger || hit.collider == _collider ||
                    hit.collider.transform.IsChildOf(transform)) continue;
                // A horizontal ray can begin on the floor portion of a
                // CompositeCollider. Ignore that upward face here so the wall
                // intersection in front of it can be selected instead.
                if (Mathf.Abs(direction.x) > 0.5f && Mathf.Abs(hit.normal.x) < 0.1f) continue;
                bool oneWay = hit.collider.GetComponentInParent<OneWayPlatform>() != null;
                if (oneWay && (!allowOneWay || dropThrough)) continue;
                if (best.collider == null || hit.distance < best.distance) best = hit;
            }
            return best;
        }

        private bool IsSlopeCollider(Collider2D collider)
        {
            return collider != null && collider.gameObject.layer == _slopeLayer;
        }

        private void UpdateRaycastOrigins()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2f);
            float sideInset = Mathf.Max(_skinWidth * 2f, 0.01f);
            _origins.SideBottomLeft = new Vector2(bounds.min.x, bounds.min.y + sideInset);
            _origins.SideBottomRight = new Vector2(bounds.max.x, bounds.min.y + sideInset);
            _origins.SideTopLeft = new Vector2(bounds.min.x, bounds.max.y - sideInset);

            _horizontalRaySpacing = (_origins.SideTopLeft.y - _origins.SideBottomLeft.y) / (_horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);

            _origins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _origins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _origins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            _origins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }
    }
}
