using System.Linq;
using Licht.Impl.Memory;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using UnityEngine;

namespace Licht.Unity.Physics
{
    public class LichtPhysicsObject : MonoBehaviour
    {
        public bool Static;
        public Vector2 VerticalColliderSize => VerticalCollider.bounds.extents;
        public Vector2 HorizontalColliderSize => HorizontalCollider.bounds.extents;

        public BoxCollider2D HorizontalCollider;
        public BoxCollider2D VerticalCollider;
        private LichtPhysics _physics;

        private string PhysicsFrameVar => $"LichtPhysicsObject_{gameObject.name}";

        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        public Vector2 Speed
        {
            get
            {
                return _physics.GetFrameVariables()
                    .Get(new FrameVariableDefinition<Vector2>(PhysicsFrameVar, () => Vector2.zero));
            }
            set
            {
                _physics.GetFrameVariables().Set(PhysicsFrameVar, value);
                _latestSpeed.Current = value;
            }
        }

        private Caterpillar<Vector2> _latestSpeed;
        public Vector2 LatestSpeed => _latestSpeed.Current;

        private void Awake()
        {
            _physics = this.GetLichtPhysics();
            _latestSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };
        }

        public void ApplySpeed(Vector2 speed)
        {
            Speed += speed;
        }

        private void OnEnable()
        {
            _physics.AddPhysicsObject(this);
        }

        private void OnDisable()
        {
            _physics.RemovePhysicsObject(this);
        }

        public CollisionResult Cast(LayerMask layerMask, Vector2 direction)
        {
            for (var index = 0; index < _collisionResults.Length; index++)
            {
                _collisionResults[index] = default;
            }

            var colliderToUse = Mathf.Abs(direction.y) > 0 ? VerticalCollider : HorizontalCollider;

            Physics2D.BoxCastNonAlloc((Vector2)transform.position + colliderToUse.offset, colliderToUse.size, 0, direction, _collisionResults,
                Speed.magnitude, layerMask);

            var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
                    && c != default)
                .ToArray();

            return new CollisionResult
            {
                Orientation = Mathf.Abs(direction.y) > 0 ? CollisionOrientation.Vertical : CollisionOrientation.Horizontal,
                Hits = hits,
                TriggeredHit = hits.Length > 0
            };
        }
    }
}
