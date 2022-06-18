using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Licht.Impl.Globals;
using Licht.Impl.Memory;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Licht.Unity.Physics
{
    [CreateAssetMenu(fileName = "LichtPhysics", menuName = "Licht/Physics/LichtPhysics", order = 1)]
    public class LichtPhysics : ScriptValue, IInitializable, IUpdateable
    {
        public float FrameMultiplier = 0.001f;
        public LayerMask ObstacleLayerMask;
        public ScriptBasicMachinery LichtPhysicsMachinery;
        public ScriptTimer ScriptTimerRef;
        private FrameVariables _frameVariables;
        private List<LichtPhysicsObject> _physicsWorld;
        private List<LichtPhysicsObject> _physicsStaticWorld;
        private List<LichtPhysicsForce> _physicsForces;
        private Dictionary<string, LichtCustomPhysicsForce> _customPhysicsForces;
        private Dictionary<LichtPhysicsObject, Caterpillar<CollisionState>> _collisionStats;
        private Dictionary<string, CollisionTrigger> _collisionTriggers;
        private HashSet<Collider2D> _semiSolids;

        public event Action<LichtPhysicsObject> OnAddPhysicsObject;
        public event Action<LichtPhysicsObject> OnRemovePhysicsObject;

        public FrameVariables GetFrameVariables()
        {
            if (_frameVariables != null) return _frameVariables;
            _frameVariables = FindObjectOfType<FrameVariables>();

            if (_frameVariables != null) return _frameVariables;
            var obj = new GameObject("frameVars");
            return _frameVariables = obj.AddComponent<FrameVariables>();
        }

        public IEnumerable<LichtPhysicsObject> GetDynamicObjectsByLayerMask(LayerMask mask)
        {
            return _physicsWorld.Where(
                obj => mask.Contains(obj.gameObject.layer));
        }

        public void AddPhysicsObject(LichtPhysicsObject obj)
        {
            OnAddPhysicsObject?.Invoke(obj);
            if (obj.Static) _physicsStaticWorld.Add(obj);
            else _physicsWorld.Add(obj);

            _collisionStats[obj] = new Caterpillar<CollisionState>
            {
                TailSize = 1
            };
        }

        public void RemovePhysicsObject(LichtPhysicsObject obj)
        {
            OnRemovePhysicsObject?.Invoke(obj);
            if (obj.Static) _physicsStaticWorld.Remove(obj);
            else _physicsWorld.Remove(obj);

            _collisionStats.Remove(obj);
        }

        public void AddPhysicsForce(LichtPhysicsForce obj)
        {
            _physicsForces.Add(obj);
        }

        public void RemovePhysicsForce(LichtPhysicsForce obj)
        {
            _physicsForces.Remove(obj);
        }

        public void AddSemiSolid(Collider2D semiSolid)
        {
            _semiSolids.Add(semiSolid);
        }

        public void RemoveSemiSolid(Collider2D semiSolid)
        {
            _semiSolids.Remove(semiSolid);
        }

        public bool IsSemiSolid(Collider2D obj)
        {
            return _semiSolids.Contains(obj);
        }

        public void AddPhysicsForce(LichtCustomPhysicsForce obj)
        {
            _customPhysicsForces[obj.Key] = obj;
        }

        public void RemovePhysicsForce(LichtCustomPhysicsForce obj)
        {
            if (_customPhysicsForces.ContainsKey(obj.Key)) _customPhysicsForces.Remove(obj.Key);
        }

        public LichtCustomPhysicsForce GetCustomPhysicsForce(string key)
        {
            return _customPhysicsForces.ContainsKey(key) ? _customPhysicsForces[key] : null;
        }

        public void Update()
        {
            foreach (var obj in _physicsWorld)
            {
                foreach (var force in _physicsForces.Where(f =>
                             f.Active &&
                             f.AffectsObjectsByLayer.Contains(obj.gameObject.layer) ||
                             (f.AffectsObjects?.Contains(obj) ?? false)))
                {
                    obj.ApplySpeed(force.Speed * FrameMultiplier * (float)ScriptTimerRef.Timer.UpdatedTimeInMilliseconds);
                }
            }

            foreach (var force in _physicsForces.Where(f => f.Temporary).ToArray())
            {
                _physicsForces.Remove(force);
            }
        }

        private CollisionResult CheckVerticals(LichtPhysicsObject obj)
        {
            // Vertical check
            var dir = obj.Speed.y > 0 ? Vector2.up : Vector2.down;
            var stats = obj.Cast(ObstacleLayerMask, dir);

            if (!stats.TriggeredHit) return stats;

            var closestHit = stats.Hits.Where(hit => hit != default && Mathf.Abs(hit.normal.y) > 0 && !Mathf.Sign(hit.normal.y).FloatEq(dir.y))
                .Select((hit, idx) => (hit.distance, idx, hit))
                .DefaultIfEmpty()
                .Min().hit;

            if (closestHit == default) return stats;

            var boxCollider = closestHit.collider as BoxCollider2D;
            if (!boxCollider?.enabled ?? false) return stats;

            var clampFix = boxCollider == null ? (0.01f - obj.VerticalCollider.offset.y) * dir.y : 0; 
            var clampPoint = boxCollider == null ? closestHit.point.y - dir.y * obj.VerticalColliderSize.y - clampFix 
                : closestHit.collider.transform.position.y + closestHit.collider.offset.y - dir.y * (boxCollider.bounds.extents.y + obj.VerticalColliderSize.y);

            if (dir == Vector2.down)
            {
                if (_semiSolids.Contains(closestHit.collider))
                {
                    var semiSolidTolerance = obj.VerticalColliderSize.y * 0.75f;
                    if (obj.transform.position.y + obj.Speed.y + semiSolidTolerance <= clampPoint) return stats;
                }

                var stopped = obj.transform.position.y + obj.Speed.y <= clampPoint;
                if (!obj.Ghost) obj.Speed = new Vector2(obj.Speed.x, stopped ? 0 : Mathf.Min(0, obj.transform.position.y - clampPoint));

                if (stopped)
                {
                    if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(obj.transform.position.x, clampPoint + clampFix);
                    stats.HitNegative = true;
                }
            }

            if (dir == Vector2.up && !_semiSolids.Contains(closestHit.collider))
            {
                var stopped = obj.transform.position.y + obj.Speed.y > clampPoint;
                if (!obj.Ghost) obj.Speed = new Vector2(obj.Speed.x, stopped ? 0 : clampPoint - obj.transform.position.y);

                if (stopped)
                {
                    if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(obj.transform.position.x, clampPoint + clampFix);
                    stats.HitPositive = true;
                }
            }

            return stats;
        }

        private static string GetCollisionTriggerName(Object collider)
        {
            return $"Collider_{collider.GetInstanceID()}";
        }

        public bool CheckCollision(Collider2D collider, out CollisionTrigger trigger)
        {
            if (!_collisionTriggers.ContainsKey(GetCollisionTriggerName(collider)))
            {
                trigger = default;
                return false;
            }

            trigger = _collisionTriggers[GetCollisionTriggerName(collider)];
            return true;
        }

        private CollisionResult[] CheckCustom(LichtPhysicsObject obj)
        {
            var results = new List<CollisionResult>();
            foreach (var check in obj.CustomCollisionChecks)
            {
                var result = obj.CustomCast(check, obj.Speed);
                if (!result.TriggeredHit) continue;

                foreach (var hit in result.Hits)
                {
                    _collisionTriggers[GetCollisionTriggerName(hit.collider)] = new CollisionTrigger
                    {
                        Actor = obj,
                        Target = hit.collider,
                        Type = CollisionTrigger.TriggerType.Custom
                    };
                }

                results.Add(result);
            }

            return results.ToArray();
        }

        private CollisionResult CheckHorizontals(LichtPhysicsObject obj)
        {
            // Horizontal check
            var dir = obj.Speed.x > 0 ? Vector2.right : Vector2.left;
            var stats = obj.Cast(ObstacleLayerMask, dir);

            if (!stats.TriggeredHit) return stats;


            var validHits = stats.Hits.Where(hit =>
                    hit != default && Mathf.Abs(hit.normal.x) > 0 && !Mathf.Sign(hit.normal.x).FloatEq(dir.x))
                .ToArray();

            var closestHit = validHits.Select((hit, idx) => (hit.distance, idx, hit))
                .DefaultIfEmpty()
                .Min().hit;

            foreach (var hit in validHits)
            {
                _collisionTriggers[GetCollisionTriggerName(hit.collider)] = new CollisionTrigger
                {
                    Actor = obj,
                    Target = hit.collider,
                    Type = CollisionTrigger.TriggerType.Obstacle
                };
            }

            if (closestHit == default) return stats;

            var boxCollider = closestHit.collider as BoxCollider2D;
            if (!boxCollider?.enabled ?? false) return stats;

            var clampFix = boxCollider == null ? 0.01f * dir.x : 0;
            var clampPoint = boxCollider == null ? closestHit.point.x - dir.x * obj.HorizontalColliderSize.x - clampFix :
                closestHit.collider.transform.position.x + closestHit.collider.offset.x - dir.x * (boxCollider.bounds.extents.x + obj.HorizontalColliderSize.x);

            if (dir == Vector2.left && !_semiSolids.Contains(closestHit.collider))
            {
                var stopped = obj.transform.position.x + obj.Speed.x < clampPoint;
                if (!obj.Ghost) obj.Speed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.Speed.y);

                if (stopped)
                {
                    if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(clampPoint + clampFix, obj.transform.position.y);
                    stats.HitNegative = true;
                }
            }

            if (dir == Vector2.right && !_semiSolids.Contains(closestHit.collider))
            {
                var stopped = obj.transform.position.x + obj.Speed.x > clampPoint;
                if (!obj.Ghost) obj.Speed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.Speed.y);

                if (stopped)
                {
                    if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(clampPoint + clampFix, obj.transform.position.y);
                    stats.HitPositive = true;
                }
            }

            return stats;
        }

        public void UpdatePositions()
        {
            _collisionTriggers.Clear();
            foreach (var obj in _physicsWorld)
            {
                var vertical = CheckVerticals(obj);
                var horizontal = CheckHorizontals(obj);
                var custom = CheckCustom(obj);

                _collisionStats[obj].Current = new CollisionState
                {
                    Horizontal = horizontal,
                    Vertical = vertical,
                    Custom = custom
                };

                obj.ImplyDirection(obj.Speed.normalized);
                obj.transform.position += (Vector3)obj.Speed;
            }

            foreach (var obj in _physicsStaticWorld)
            {
                var vertical = CheckVerticals(obj);
                var horizontal = CheckHorizontals(obj);
                var custom = CheckCustom(obj);

                _collisionStats[obj].Current = new CollisionState
                {
                    Horizontal = horizontal,
                    Vertical = vertical,
                    Custom = custom
                };
            }
        }

        public override object Value => this;

        public void Initialize()
        {
            _frameVariables = GetFrameVariables();
            _physicsWorld = new List<LichtPhysicsObject>();
            _physicsStaticWorld = new List<LichtPhysicsObject>();
            _physicsForces = new List<LichtPhysicsForce>();
            _customPhysicsForces = new Dictionary<string, LichtCustomPhysicsForce>();
            _collisionStats = new Dictionary<LichtPhysicsObject, Caterpillar<CollisionState>>();
            _collisionTriggers = new Dictionary<string, CollisionTrigger>();
            _semiSolids = new HashSet<Collider2D>();
        }

        public CollisionState GetCollisionState(LichtPhysicsObject obj)
        {
            return !_collisionStats.ContainsKey(obj) ? default : _collisionStats[obj].Current;
        }

        public void UnblockCustomPhysicsForceForObject(MonoBehaviour src, LichtPhysicsObject obj, string force)
        {
            if (!_customPhysicsForces.ContainsKey(force)) return;

            _customPhysicsForces[force].UnblockForceFor(src, obj);
        }

        public void BlockCustomPhysicsForceForObject(MonoBehaviour src, LichtPhysicsObject obj, string force)
        {
            if (!_customPhysicsForces.ContainsKey(force)) return;

            _customPhysicsForces[force].BlockForceFor(src, obj);
        }
    }
}
