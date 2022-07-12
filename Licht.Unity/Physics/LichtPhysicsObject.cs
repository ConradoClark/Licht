using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Memory;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

namespace Licht.Unity.Physics
{
    public class LichtPhysicsObject : MonoBehaviour
    {
        public bool Debug;
        public bool Static;
        public bool Ghost;

        [Serializable]
        public class TriggerDefinitions
        {
            public ScriptIdentifier TriggerName;
            public bool Triggered;
        }

        public TriggerDefinitions[] PhysicsTriggers;
        public LichtPhysicsCollisionDetector[] CollisionDetectors;
        public List<Collider2D> AdditionalColliders;

        private LichtPhysics _physics;
        private string PhysicsFrameVar => $"LichtPhysicsObject_{gameObject.GetInstanceID()}";
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        private Dictionary<Type, object> _customObjects;

        public bool GetPhysicsTrigger(ScriptIdentifier identifier)
        {
            return PhysicsTriggers?.FirstOrDefault(t => t.TriggerName == identifier)?.Triggered ?? false;
        }

        public void SetPhysicsTrigger(ScriptIdentifier identifier, bool value)
        {
            var trigger = PhysicsTriggers?.FirstOrDefault(t => t.TriggerName == identifier);
            if (trigger == null) throw new Exception($"Trigger {identifier.Name} not defined in Physics Object.");

            trigger.Triggered = value;
        }

        public void AddCustomObject<T>(T obj) where T : class
        {
            _customObjects ??= new Dictionary<Type, object>();
            _customObjects[typeof(T)] = obj;
        }

        public void RemoveCustomObject<T>() where T: class
        {
            _customObjects ??= new Dictionary<Type, object>();
            if (!_customObjects.ContainsKey(typeof(T))) return;

            _customObjects.Remove(typeof(T));
        }

        public bool TryGetCustomObject<T>(out T obj) where T : class
        {
            if (!_customObjects.ContainsKey(typeof(T)))
            {
                obj = default;
                return false;
            }

            obj = _customObjects[typeof(T)] as T;
            return true;
        }

        public bool HasCustomObjectOfType(Type type)
        {
            return _customObjects.ContainsKey(type);
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
            _customObjects ??= new Dictionary<Type, object>();
            _physics = this.GetLichtPhysics();
            _latestSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            _latestDirection = new Caterpillar<Vector2>
            {
                TailSize = 1
            };


            foreach (var additionalCollider in AdditionalColliders)
            {
                _physics.RegisterCollider(additionalCollider, this);
            }
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

        public void CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType type)
        {
            foreach (var detector in CollisionDetectors.Where(cd => cd.DetectorType 
                                                                    == type))
            {
                detector.AttachPhysicsObject(this);
                if (detector.IsBlocked || !detector.isActiveAndEnabled) continue;

                detector.CheckCollision();

                var result = detector.Clamp();
                if (type == LichtPhysicsCollisionDetector.CollisionDetectorType.PreUpdate) CalculatedSpeed = result;
                else transform.position = result;
            }
        }

        //public CollisionResult CustomCast(CustomCollision customCollision, Vector2 direction)
        //{
        //    for (var index = 0; index < _collisionResults.Length; index++)
        //    {
        //        _collisionResults[index] = default;
        //    }

        //    Physics2D.BoxCastNonAlloc((Vector2)transform.position + customCollision.ColliderToUse.offset, customCollision.ColliderToUse.size,
        //        0, direction, _collisionResults,
        //        Speed.magnitude, customCollision.CollisionLayerMask);

        //    var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
        //            && c != default)
        //        .ToArray();

        //    return new CollisionResult
        //    {
        //        Orientation = CollisionOrientation.Undefined,
        //        Hits = hits,
        //        TriggeredHit = hits.Length > 0
        //    };
        //}

        //public CollisionResult Cast2(LayerMask layerMask, Vector2 direction)
        //{
        //    for (var index = 0; index < _collisionResults.Length; index++)
        //    {
        //        _collisionResults[index] = default;
        //    }

        //    if (!HorizontalCollider.enabled)
        //    {
        //        return new CollisionResult
        //        {
        //            Orientation = Mathf.Abs(direction.y) > 0
        //                ? CollisionOrientation.Vertical
        //                : CollisionOrientation.Horizontal,
        //            Hits = Array.Empty<RaycastHit2D>(),
        //            TriggeredHit = false
        //        };
        //    }

        //    var origin = (Vector2)transform.position
        //                 + new Vector2(HorizontalCollider.offset.x, HorizontalCollider.offset.y)
        //                 + new Vector2(_physics.CollisionOffset * direction.x, _physics.CollisionOffset * direction.y);

        //    var size = HorizontalCollider.size;

        //    var rayDistance = Mathf.Max(_physics.MinRayCastSize, (CalculatedSpeed + size * 2f * direction).magnitude);

        //    Physics2D.BoxCastNonAlloc(origin, size, 0, direction, _collisionResults, rayDistance, layerMask);

        //    if (Debug)
        //    {
        //        DebugDraw(origin, size, 0, direction, rayDistance, _collisionResults[0]);
        //    }

        //    var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
        //            && c != default)
        //        .ToArray();

        //    return new CollisionResult
        //    {
        //        Orientation = Mathf.Abs(direction.y) > 0 ? CollisionOrientation.Vertical : CollisionOrientation.Horizontal,
        //        Hits = hits,
        //        Detected = hits.Length > 0,
        //        Direction = direction,
        //    };
        //}

        //public CollisionResult Cast(LayerMask layerMask, BoxCollider2D colliderToUse, Vector2 direction)
        //{
        //    for (var index = 0; index < _collisionResults.Length; index++)
        //    {
        //        _collisionResults[index] = default;
        //    }

        //    if (!colliderToUse.enabled)
        //    {
        //        return new CollisionResult
        //        {
        //            Orientation = colliderToUse == VerticalCollider
        //                ? CollisionOrientation.Vertical
        //                : CollisionOrientation.Horizontal,
        //            Hits = Array.Empty<RaycastHit2D>(),
        //            TriggeredHit = false
        //        };
        //    }

        //    var horizontalFactor = colliderToUse == HorizontalCollider ? 1 : 0;
        //    var verticalFactor = colliderToUse == VerticalCollider ? 1 : 0;

        //    var origin = (Vector2)transform.position
        //                 + new Vector2(colliderToUse.offset.x, colliderToUse.offset.y)
        //                 + new Vector2(_physics.CollisionOffset * -direction.x, _physics.CollisionOffset * -direction.y);

        //    var size = colliderToUse.size + new Vector2(colliderToUse == HorizontalCollider && direction.x == 0 ? _physics.CollisionOffset : 0,
        //        colliderToUse == VerticalCollider && direction.y == 0 ? _physics.CollisionOffset : 0);

        //    /* - new Vector2(Mathf.Abs(direction.y) * _physics.CollisionOffset, Mathf.Abs(direction.x) * _physics.CollisionOffset)
        //        + new Vector2(colliderToUse == HorizontalCollider && direction.x == 0 ? _physics.CollisionOffset : 0, 
        //            colliderToUse == VerticalCollider && direction.y == 0 ? _physics.CollisionOffset : 0);*/

        //    var distanceMagnitude = new Vector2(CalculatedSpeed.x * Mathf.Abs(direction.x) * horizontalFactor,
        //        CalculatedSpeed.y * Mathf.Abs(direction.y) * verticalFactor).magnitude;

        //    Physics2D.BoxCastNonAlloc(origin, size, 0, direction, _collisionResults,
        //        distanceMagnitude + _physics.MinRayCastSize, layerMask);

        //    //if (Debug && colliderToUse == VerticalCollider)
        //    //{
        //    //    DebugDraw(origin, size, 0, direction, distanceMagnitude + _physics.CollisionOffset, _collisionResults[0]);
        //    //}

        //    var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
        //            && c != default)
        //        .ToArray();

        //    return new CollisionResult
        //    {
        //        Orientation = Mathf.Abs(direction.y) > 0 ? CollisionOrientation.Vertical : CollisionOrientation.Horizontal,
        //        Hits = hits,
        //        TriggeredHit = hits.Length > 0
        //    };
        //}

        //private void DebugDraw(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance,
        //    RaycastHit2D hit)
        //{
        //    var w = size.x * 0.5f;
        //    var h = size.y * 0.5f;
        //    var p1 = new Vector2(-w, h);
        //    var p2 = new Vector2(w, h);
        //    var p3 = new Vector2(w, -h);
        //    var p4 = new Vector2(-w, -h);

        //    var q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        //    p1 = q * p1;
        //    p2 = q * p2;
        //    p3 = q * p3;
        //    p4 = q * p4;

        //    p1 += origin;
        //    p2 += origin;
        //    p3 += origin;
        //    p4 += origin;

        //    var realDistance = direction.normalized * distance;
        //    var p5 = p1 + realDistance;
        //    var p6 = p2 + realDistance;
        //    var p7 = p3 + realDistance;
        //    var p8 = p4 + realDistance;


        //    //Drawing the cast
        //    var castColor = hit ? Color.red : Color.green;
        //    UnityEngine.Debug.DrawLine(p1, p2, castColor);
        //    UnityEngine.Debug.DrawLine(p2, p3, castColor);
        //    UnityEngine.Debug.DrawLine(p3, p4, castColor);
        //    UnityEngine.Debug.DrawLine(p4, p1, castColor);

        //    UnityEngine.Debug.DrawLine(p5, p6, castColor);
        //    UnityEngine.Debug.DrawLine(p6, p7, castColor);
        //    UnityEngine.Debug.DrawLine(p7, p8, castColor);
        //    UnityEngine.Debug.DrawLine(p8, p5, castColor);

        //    UnityEngine.Debug.DrawLine(p1, p5, Color.grey);
        //    UnityEngine.Debug.DrawLine(p2, p6, Color.grey);
        //    UnityEngine.Debug.DrawLine(p3, p7, Color.grey);
        //    UnityEngine.Debug.DrawLine(p4, p8, Color.grey);
        //    if (hit)
        //    {
        //        UnityEngine.Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
        //    }
        //}
    }
}
