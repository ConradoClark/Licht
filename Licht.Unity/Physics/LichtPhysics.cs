using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Globals;
using Licht.Impl.Memory;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics
{
    [CreateAssetMenu(fileName = "LichtPhysics", menuName = "Licht/Physics/LichtPhysics", order = 1)]
    public class LichtPhysics : ScriptableValue, IInitializable, IUpdateable
    {
        public float FrameMultiplier = 0.001f;
        public LayerMask ObstacleLayerMask;
        public BasicMachineryScriptable LichtPhysicsMachinery;
        public TimerScriptable TimerRef;
        private FrameVariables _frameVariables;
        private List<LichtPhysicsObject> _physicsWorld;
        private List<LichtPhysicsObject> _physicsStaticWorld;
        private List<LichtPhysicsForce> _physicsForces;
        private Dictionary<string, LichtCustomPhysicsForce> _customPhysicsForces;
        private Dictionary<LichtPhysicsObject, Caterpillar<CollisionState>> _collisionStats;

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
            if (obj.Static) _physicsStaticWorld.Add(obj);
            else _physicsWorld.Add(obj);

            _collisionStats[obj] = new Caterpillar<CollisionState>
            {
                TailSize = 1
            };
        }

        public void RemovePhysicsObject(LichtPhysicsObject obj)
        {
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
                    obj.ApplySpeed(force.Speed * FrameMultiplier * (float)TimerRef.Timer.UpdatedTimeInMilliseconds);
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

            var closestHit = stats.Hits.Where(hit => hit != default && Mathf.Abs(hit.normal.y) > 0)
                .Select((hit, idx) => (hit.distance, idx, hit))
                .DefaultIfEmpty()
                .Min().hit;

            if (closestHit == default) return stats;

            var boxCollider = closestHit.collider as BoxCollider2D;
            if (!boxCollider?.enabled ?? false) return stats;

            var clampPoint = closestHit.collider.transform.position.y + closestHit.collider.offset.y
                             - dir.y * ((boxCollider == null ? 0 : boxCollider.bounds.extents.y) + obj.VerticalColliderSize.y);

            if (dir == Vector2.down && !Mathf.Sign(closestHit.normal.y).FloatEq(dir.y))
            {
                var stopped = obj.transform.position.y + obj.Speed.y <= clampPoint;
                obj.Speed = new Vector2(obj.Speed.x, stopped ? 0 : Mathf.Min(0, obj.transform.position.y - clampPoint));

                if (stopped)
                {
                    obj.transform.position = new Vector2(obj.transform.position.x, clampPoint);
                    stats.HitNegative = true;
                }
            }

            if (dir == Vector2.up && !Mathf.Sign(closestHit.normal.y).FloatEq(dir.y))
            {
                var stopped = obj.transform.position.y + obj.Speed.y > clampPoint;
                obj.Speed = new Vector2(obj.Speed.x, stopped ? 0 : clampPoint - obj.transform.position.y);

                if (stopped)
                {
                    obj.transform.position = new Vector2(obj.transform.position.x, clampPoint);
                    stats.HitPositive = true;
                }
            }

            return stats;
        }

        private CollisionResult CheckHorizontals(LichtPhysicsObject obj)
        {
            // Horizontal check
            var dir = obj.Speed.x > 0 ? Vector2.right : Vector2.left;
            var stats = obj.Cast(ObstacleLayerMask, dir);

            if (!stats.TriggeredHit) return stats;

            // fix closestHit
            var closestHit = stats.Hits.Where(hit => hit != default && Mathf.Abs(hit.normal.x) > 0)
                .Select((hit, idx) => (hit.distance, idx, hit))
                .DefaultIfEmpty()
                .Min().hit;

            if (closestHit == default) return stats;

            var boxCollider = closestHit.collider as BoxCollider2D;
            if (!boxCollider?.enabled ?? false) return stats;
            var clampPoint = closestHit.collider.transform.position.x + closestHit.collider.offset.x -
                             dir.x * ((boxCollider == null ? 0 : boxCollider.bounds.extents.x) +
                                      obj.HorizontalColliderSize.x);

            if (dir == Vector2.left && !Mathf.Sign(closestHit.normal.x).FloatEq(dir.x))
            {
                var stopped = obj.transform.position.x + obj.Speed.x < clampPoint;
                obj.Speed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.Speed.y);

                if (stopped)
                {
                    obj.transform.position = new Vector2(clampPoint, obj.transform.position.y);
                    stats.HitNegative = true;
                }
            }

            if (dir == Vector2.right && !Mathf.Sign(closestHit.normal.x).FloatEq(dir.x))
            {
                var stopped = obj.transform.position.x + obj.Speed.x > clampPoint;
                obj.Speed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.Speed.y);

                if (stopped)
                {
                    obj.transform.position = new Vector2(clampPoint, obj.transform.position.y);
                    stats.HitPositive = true;
                }
            }

            return stats;
        }

        public void UpdatePositions()
        {
            foreach (var obj in _physicsWorld)
            {
                var vertical = CheckVerticals(obj);
                var horizontal = CheckHorizontals(obj);

                _collisionStats[obj].Current = new CollisionState
                {
                    Horizontal = horizontal,
                    Vertical = vertical
                };
                obj.transform.position += (Vector3)obj.Speed;
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
