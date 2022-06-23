using System;
using System.Collections.Generic;
using System.IO;
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
        public bool Ghost;
        public Vector2 VerticalColliderSize => VerticalCollider.bounds.extents;
        public Vector2 HorizontalColliderSize => HorizontalCollider.bounds.extents;

        public BoxCollider2D HorizontalCollider;
        public BoxCollider2D VerticalCollider;

        [Serializable]
        public struct CustomCollision
        {
            public LayerMask CollisionLayerMask;
            public string CollisionName;
            public BoxCollider2D ColliderToUse;
        }

        public CustomCollision[] CustomCollisionChecks;

        private LichtPhysics _physics;
        private string PhysicsFrameVar => $"LichtPhysicsObject_{gameObject.name}";
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        private Dictionary<Type, object> _customObjects;

        public void AddCustomObject<T>(T obj) where T:class
        {
            _customObjects[typeof(T)] = obj;
        }

        public bool TryGetCustomObject<T>(out T obj) where T:class
        {
            if (!_customObjects.ContainsKey(typeof(T)))
            {
                obj = default;
                return false;
            }

            obj = _customObjects[typeof(T)] as T;
            return true;
        }

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

        public Vector2 CalculatedSpeed;

        private Caterpillar<Vector2> _latestSpeed;
        public Vector2 LatestSpeed => _latestSpeed.Current;

        private Caterpillar<Vector2> _latestDirection;
        public Vector2 LatestDirection => _latestDirection.Current;

        private void Awake()
        {
            _customObjects = new Dictionary<Type, object>();
            _physics = this.GetLichtPhysics();
            _latestSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            _latestDirection = new Caterpillar<Vector2>
            {
                TailSize = 1
            };
        }

        public void ImplyDirection(Vector2 direction)
        {
            _latestDirection.Current = new Vector2(Mathf.Abs(direction.x) > 0 ? direction.x : _latestDirection.Current.x,
                Mathf.Abs(direction.y) > 0 ? direction.y : _latestDirection.Current.y);
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

        public CollisionResult CustomCast(CustomCollision customCollision, Vector2 direction)
        {
            for (var index = 0; index < _collisionResults.Length; index++)
            {
                _collisionResults[index] = default;
            }

            Physics2D.BoxCastNonAlloc((Vector2)transform.position + customCollision.ColliderToUse.offset, customCollision.ColliderToUse.size,
                0, direction, _collisionResults,
                Speed.magnitude, customCollision.CollisionLayerMask);

            var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
                    && c != default)
                .ToArray();

            return new CollisionResult
            {
                Orientation = CollisionOrientation.Undefined,
                Hits = hits,
                TriggeredHit = hits.Length > 0
            };
        }

        public CollisionResult Cast(LayerMask layerMask, Vector2 direction)
        {
            for (var index = 0; index < _collisionResults.Length; index++)
            {
                _collisionResults[index] = default;
            }

            var colliderToUse = Mathf.Abs(direction.y) > 0 ? VerticalCollider : HorizontalCollider;

            if (!colliderToUse.enabled)
            {
                return new CollisionResult
                {
                    Orientation = Mathf.Abs(direction.y) > 0
                        ? CollisionOrientation.Vertical
                        : CollisionOrientation.Horizontal,
                    Hits = Array.Empty<RaycastHit2D>(),
                    TriggeredHit = false
                };
            }

            var origin = (Vector2)transform.position
                         + new Vector2(colliderToUse.offset.x, colliderToUse.offset.y)
                         + new Vector2(_physics.CollisionOffset * -direction.x,
                             _physics.CollisionOffset * -direction.y);

            var size = colliderToUse.size - new Vector2(Mathf.Abs(direction.y) * 0.1f, Mathf.Abs(direction.x) * 0.1f);

            Physics2D.BoxCastNonAlloc(origin, size, 0, direction, _collisionResults,
                CalculatedSpeed.magnitude + _physics.CollisionOffset, layerMask);

            if (_physics.Debug)
            {
                DebugDraw(origin, size, 0, direction, CalculatedSpeed.magnitude + _physics.CollisionOffset, _collisionResults[0]);
            }

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

        private void DebugDraw(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance,
            RaycastHit2D hit)
        {
            var w = size.x * 0.5f;
            var h = size.y * 0.5f;
            var p1 = new Vector2(-w, h);
            var p2 = new Vector2(w, h);
            var p3 = new Vector2(w, -h);
            var p4 = new Vector2(-w, -h);

            var q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            p1 = q * p1;
            p2 = q * p2;
            p3 = q * p3;
            p4 = q * p4;

            p1 += origin;
            p2 += origin;
            p3 += origin;
            p4 += origin;

            var realDistance = direction.normalized * distance;
            var p5 = p1 + realDistance;
            var p6 = p2 + realDistance;
            var p7 = p3 + realDistance;
            var p8 = p4 + realDistance;


            //Drawing the cast
            var castColor = hit ? Color.red : Color.green;
            Debug.DrawLine(p1, p2, castColor);
            Debug.DrawLine(p2, p3, castColor);
            Debug.DrawLine(p3, p4, castColor);
            Debug.DrawLine(p4, p1, castColor);

            Debug.DrawLine(p5, p6, castColor);
            Debug.DrawLine(p6, p7, castColor);
            Debug.DrawLine(p7, p8, castColor);
            Debug.DrawLine(p8, p5, castColor);

            Debug.DrawLine(p1, p5, Color.grey);
            Debug.DrawLine(p2, p6, Color.grey);
            Debug.DrawLine(p3, p7, Color.grey);
            Debug.DrawLine(p4, p8, Color.grey);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
            }
        }
    }
}
