using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Memory;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Licht.Unity.Physics
{
    [CreateAssetMenu(fileName = "LichtPhysics", menuName = "Licht/Physics/LichtPhysics", order = 1)]
    public class LichtPhysics : ScriptValue, IInitializable, IUpdateable
    {
        public bool Debug;
        public float MinRayCastSize = 1f;
        public float CollisionOffset = 0.01f;
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
                    obj.ApplySpeed(force.Speed);
                }
            }

            foreach (var force in _physicsForces.Where(f => f.Temporary).ToArray())
            {
                _physicsForces.Remove(force);
            }
        }

        //private CollisionResult CheckVerticals(LichtPhysicsObject obj)
        //{
        //    // Vertical check
        //    var dir = obj.CalculatedSpeed.y > 0 ? Vector2.up : obj.CalculatedSpeed.y < 0 ? Vector2.down : Vector2.zero;
        //    var stats = obj.Cast(ObstacleLayerMask, obj.VerticalCollider, dir);

        //    if (!stats.TriggeredHit)
        //        return stats;

        //    var closestHit = stats.Hits.Where(hit => hit != default)
        //        .Select((hit, idx) => (1 - Mathf.Abs(hit.normal.y), hit.distance, idx, hit))
        //        .DefaultIfEmpty()
        //        .Min().hit;

        //    if (closestHit == default)
        //        return stats;

        //    var boxCollider = closestHit.collider as BoxCollider2D;
        //    if (!boxCollider?.enabled ?? false) return stats;

        //    var clampFix = boxCollider == null ? 0.01f * dir.y : 0;
        //    var clampPoint = boxCollider == null ? closestHit.point.y - dir.y * obj.VerticalColliderSize.y - clampFix
        //        : closestHit.collider.transform.position.y + closestHit.collider.offset.y - dir.y * (boxCollider.bounds.extents.y + obj.VerticalColliderSize.y);

        //    if (dir == Vector2.zero)
        //    {
        //        stats.HitNegative = closestHit.point.y < obj.transform.position.y;
        //        stats.HitPositive = closestHit.point.y > obj.transform.position.y;
        //    }

        //    if (Vector2.Distance(closestHit.point, new Vector2(closestHit.point.x, clampPoint)) >
        //        obj.VerticalCollider.size.magnitude * 0.5f)
        //        return stats;

        //    if (dir == Vector2.down)
        //    {
        //        if (_semiSolids.Contains(closestHit.collider))
        //        {
        //            var semiSolidTolerance = obj.VerticalColliderSize.y * 0.75f;
        //            if (obj.transform.position.y + obj.CalculatedSpeed.y + semiSolidTolerance <= clampPoint) return stats;
        //        }

        //        var stopped = obj.transform.position.y + obj.CalculatedSpeed.y <= clampPoint;
        //        var reflection = Vector2.Reflect(obj.CalculatedSpeed.normalized, closestHit.normal).y;

        //        if (!obj.Ghost) obj.CalculatedSpeed = new Vector2(obj.CalculatedSpeed.x, stopped ? 0 : Mathf.Min(0, obj.transform.position.y - clampPoint));

        //        if (stopped && Mathf.Abs(reflection) > 0f)
        //        {
        //            if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(obj.transform.position.x, clampPoint + clampFix);
        //            stats.HitNegative = true;
        //        }
        //    }

        //    if (dir == Vector2.up && !_semiSolids.Contains(closestHit.collider))
        //    {
        //        var stopped = obj.transform.position.y + obj.CalculatedSpeed.y >= clampPoint;
        //        var reflection = Vector2.Reflect(obj.CalculatedSpeed.normalized, closestHit.normal).y;

        //        if (!obj.Ghost) obj.CalculatedSpeed = new Vector2(obj.CalculatedSpeed.x, stopped ? 0 :
        //            Mathf.Min(obj.CalculatedSpeed.y, Mathf.Max(0, clampPoint - obj.transform.position.y)));

        //        if (stopped && Mathf.Abs(reflection) > 0f)
        //        {
        //            if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(obj.transform.position.x, clampPoint + clampFix);
        //            stats.HitPositive = true;
        //        }
        //    }

        //    return stats;
        //}

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
                var result = obj.CustomCast(check, obj.CalculatedSpeed);
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

        private CollisionResult CheckDirection(Vector2 direction, LichtPhysicsObject obj)
        {
            var stats = obj.Cast2(ObstacleLayerMask, direction);

            var validHits = stats.Hits.Where(hit => hit != default && direction * hit.normal != Vector2.zero)
                .ToArray();

            var closestHit = validHits
                .Select((hit, idx) => (hit.distance, idx, hit))
                .DefaultIfEmpty()
                .Min().hit;

            stats.Hits = validHits;
            stats.Detected = stats.Hits.Length > 0;

            if (closestHit == default) return stats;

            stats.ClosestHit = closestHit;

            foreach (var hit in validHits)
            {
                _collisionTriggers[GetCollisionTriggerName(hit.collider)] = new CollisionTrigger
                {
                    Actor = obj,
                    Target = hit.collider,
                    Type = CollisionTrigger.TriggerType.Obstacle
                };
            }

            var clampPoint = (direction * closestHit.point) - obj.HorizontalColliderSize;
            stats.ClampPoint = clampPoint;
            if (obj.Debug)
            {
                DrawEllipse(clampPoint, Vector3.forward, Vector3.up, 0.15f, 0.15f, 32, Color.magenta, 0);
            }

            var original = (obj.HorizontalCollider.offset + (Vector2) obj.transform.position) * direction;

            var test = Vector2.Distance(clampPoint, original);
            var travel = (obj.HorizontalCollider.offset + (Vector2)obj.transform.position + obj.CalculatedSpeed) * direction;
            var distance = Vector2.Distance(clampPoint, travel);

            stats.OriginalDistance = test;
            stats.TravelDistance = distance;

            //if (test < CollisionOffset)
            //{
            //    stats.TriggeredHit = true;
            //}

            //if (!(distance < test)) return stats;

            //if (obj.Debug)
            //{
            //    DrawEllipse(obj.CalculatedSpeed * direction + (Vector2)obj.transform.position, Vector3.forward, Vector3.up, 0.15f, 0.15f, 32, Color.red, 1f);
            //}
            
            //obj.CalculatedSpeed = Vector2.ClampMagnitude(obj.CalculatedSpeed, test);
            //if (travel.x - original.x < 0)
            //{
            //    obj.CalculatedSpeed = new Vector2(Mathf.Clamp(obj.CalculatedSpeed.x, 
            //        travel.x < original.x ? 0 : original.x - travel.x,
            //        travel.x > original.x ? original.x - travel.x : 0), obj.CalculatedSpeed.y);
            //    stats.TriggeredHit = true;
            //}

            //if (travel.y - original.y < 0)
            //{
            //    obj.CalculatedSpeed = new Vector2(obj.CalculatedSpeed.x, Mathf.Clamp(obj.CalculatedSpeed.y,
            //        travel.y < original.y ? travel.y - original.y : 0,
            //        travel.y > original.y ? original.y - travel.y : 0));
            //    stats.TriggeredHit = true;
            //}

            return stats;
        }

        private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
        {
            var angle = 0f;
            var rot = Quaternion.LookRotation(forward, up);
            var lastPoint = Vector3.zero;
            var thisPoint = Vector3.zero;

            for (var i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                if (i > 0)
                {
                    UnityEngine.Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }

        //private CollisionResult CheckHorizontals(LichtPhysicsObject obj)
        //{
        //    // Horizontal check
        //    var dir = obj.CalculatedSpeed.x > 0 ? Vector2.right : Vector2.left;
        //    var stats = obj.Cast(ObstacleLayerMask, obj.HorizontalCollider, dir);

        //    if (!stats.TriggeredHit) return stats;

        //    var validHits = stats.Hits.Where(hit => hit != default)
        //        .ToArray();

        //    var closestHit = validHits.Select((hit, idx) => (hit.distance, idx, hit))
        //        .DefaultIfEmpty()
        //        .Min().hit;

        //    foreach (var hit in validHits)
        //    {
        //        _collisionTriggers[GetCollisionTriggerName(hit.collider)] = new CollisionTrigger
        //        {
        //            Actor = obj,
        //            Target = hit.collider,
        //            Type = CollisionTrigger.TriggerType.Obstacle
        //        };
        //    }

        //    if (closestHit == default) return stats;

        //    var boxCollider = closestHit.collider as BoxCollider2D;
        //    if (!boxCollider?.enabled ?? false) return stats;

        //    var clampFix = boxCollider == null ? 0.01f * dir.x : 0;
        //    var clampPoint = boxCollider == null ? closestHit.point.x - dir.x * obj.HorizontalColliderSize.x - clampFix :
        //        closestHit.collider.transform.position.x + closestHit.collider.offset.x - dir.x * (boxCollider.bounds.extents.x + obj.HorizontalColliderSize.x);

        //    if (Vector2.Distance(closestHit.point, new Vector2(clampPoint, closestHit.point.y)) >
        //        obj.HorizontalCollider.size.magnitude * 0.5f) return stats;

        //    if (dir == Vector2.left && !_semiSolids.Contains(closestHit.collider))
        //    {
        //        var stopped = obj.transform.position.x + obj.CalculatedSpeed.x < clampPoint;
        //        if (!obj.Ghost) obj.CalculatedSpeed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.CalculatedSpeed.y);

        //        if (stopped && Mathf.Abs(Vector2.Reflect(obj.CalculatedSpeed.normalized, closestHit.normal).x) > 0f)
        //        {
        //            if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(clampPoint + clampFix, obj.transform.position.y);
        //            stats.HitNegative = true;
        //        }
        //    }

        //    if (dir == Vector2.right && !_semiSolids.Contains(closestHit.collider))
        //    {
        //        var stopped = obj.transform.position.x + obj.CalculatedSpeed.x > clampPoint;
        //        if (!obj.Ghost) obj.CalculatedSpeed = new Vector2(stopped ? 0 : clampPoint - obj.transform.position.x, obj.CalculatedSpeed.y);

        //        if (stopped && Mathf.Abs(Vector2.Reflect(obj.CalculatedSpeed.normalized, closestHit.normal).x) > 0f)
        //        {
        //            if (!obj.Static && !obj.Ghost) obj.transform.position = new Vector2(clampPoint + clampFix, obj.transform.position.y);
        //            stats.HitPositive = true;
        //        }
        //    }

        //    return stats;
        //}

        private CollisionResult HandleHorizontal(CollisionResult result, LichtPhysicsObject obj)
        {

            if (obj.transform.position.x < 0 && obj.CalculatedSpeed.x + obj.transform.position.x > 0 && !result.Detected)
            {

            }

            if (!result.Detected) return result;
            var diff = result.TravelDistance - result.OriginalDistance;

            var clamp = result.ClampPoint.x - obj.transform.position.x * result.Direction.x;

            if (diff < 0 || diff > clamp || clamp < CollisionOffset)
            {
                obj.CalculatedSpeed = new Vector2(
                    Mathf.Clamp(obj.CalculatedSpeed.x, result.Direction.x > 0 ? obj.CalculatedSpeed.x : -clamp,
                        result.Direction.x > 0 ? clamp : obj.CalculatedSpeed.x)
                    ,  obj.CalculatedSpeed.y);

                UnityEngine.Debug.Log("tried to stop");

                if (Mathf.Abs(clamp) < CollisionOffset)
                {
                    UnityEngine.Debug.Log("collided");
                    result.TriggeredHit = true;
                }
            }

            return result;
        }

        private CollisionResult HandleGround(CollisionResult result, LichtPhysicsObject obj)
        {
            if (!result.Detected || result.ClosestHit.normal.y > 0 ) return result;
            var diff = result.TravelDistance - result.OriginalDistance;

            if (diff <= 0 || result.TravelDistance < CollisionOffset)
            {
                obj.CalculatedSpeed = new Vector2(obj.CalculatedSpeed.x,
                    Mathf.Clamp(obj.CalculatedSpeed.y, result.Direction.y > 0 ? 0 : diff,
                        result.Direction.y > 0 ? -diff : 0));

                if (result.TravelDistance < CollisionOffset)
                {
                    result.TriggeredHit = true;
                }
            }

            return result;
        }

        private CollisionResult HandleUp(CollisionResult result, LichtPhysicsObject obj)
        {
            if (!result.Detected || result.ClosestHit.normal.y < 0) return result;
            var diff = result.TravelDistance - result.OriginalDistance;

            if (diff <= 0 || result.TravelDistance < CollisionOffset)
            {
                obj.CalculatedSpeed = new Vector2(obj.CalculatedSpeed.x,
                    Mathf.Clamp(obj.CalculatedSpeed.y, result.Direction.y > 0 ? 0 : diff,
                        result.Direction.y > 0 ? -diff : 0));

                if (result.TravelDistance < CollisionOffset)
                {
                    result.TriggeredHit = true;
                }
            }

            return result;
        }

        public void UpdatePositions()
        {
            _collisionTriggers.Clear();
            foreach (var obj in _physicsWorld)
            {
                obj.CalculatedSpeed = obj.Speed * FrameMultiplier * (float)ScriptTimerRef.Timer.UpdatedTimeInMilliseconds;

                var right = CheckDirection(Vector2.right, obj);
                var left = CheckDirection(Vector2.left, obj);
                var up = CheckDirection(Vector2.up, obj);
                var down = CheckDirection(Vector2.down, obj);


                right = HandleHorizontal(right, obj);
                // left = HandleHorizontal(left,obj);
               // down = HandleGround(down, obj);

                var custom = CheckCustom(obj);

                _collisionStats[obj].Current = new CollisionState
                {
                    Left = left,
                    Right = right,
                    Up = up,
                    Down = down,
                    Custom = custom
                };

                obj.ImplyDirection(obj.CalculatedSpeed.normalized);
                obj.transform.position += (Vector3)obj.CalculatedSpeed;
            }

            foreach (var obj in _physicsStaticWorld)
            {
                var right = CheckDirection(Vector2.right, obj);
                var left = CheckDirection(Vector2.left, obj);
                var up = CheckDirection(Vector2.up, obj);
                var down = CheckDirection(Vector2.down, obj);

                var custom = CheckCustom(obj);

                _collisionStats[obj].Current = new CollisionState
                {
                    Left = left,
                    Right = right,
                    Up = up,
                    Down = down,
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
