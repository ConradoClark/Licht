using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace Licht.Unity.Physics
{
    [CreateAssetMenu(fileName = "LichtPhysics", menuName = "Licht/Physics/LichtPhysics", order = 1)]
    public class LichtPhysics : ScriptValue, ICanInitialize, IUpdateable
    {
        public bool Debug;
        public float MinRayCastSize = 1f;
        public float CollisionOffset = 0.01f;
        public float FrameMultiplier = 0.001f;
        public float PhysicsUpdateFrequency = 5 / 60f;
        public float MaxDistanceBeforeCollisionCheck;
        public LayerMask ObstacleLayerMask;
        public ScriptBasicMachinery LichtPhysicsMachinery;
        public ScriptTimer ScriptTimerRef;
        private FrameVariables _frameVariables;
        private List<LichtPhysicsObject> _physicsWorld;
        private List<LichtPhysicsObject> _physicsStaticWorld;
        private List<LichtPhysicsForce> _physicsForces;
        private Dictionary<string, LichtCustomPhysicsForce> _customPhysicsForces;
        private HashSet<Collider2D> _semiSolids;

        private Dictionary<Collider2D, LichtPhysicsObject> _colliderRegistry;

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
        }

        public void RemovePhysicsObject(LichtPhysicsObject obj)
        {
            OnRemovePhysicsObject?.Invoke(obj);
            if (obj.Static) _physicsStaticWorld.Remove(obj);
            else _physicsWorld.Remove(obj);
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
            if (Physics2D.simulationMode == SimulationMode2D.Script){
                var timer = PhysicsUpdateFrequency;
                timer -= (float) ScriptTimerRef.Timer.UpdatedTimeInMilliseconds;

                // Catch up with the game time.
                // Advance the physics simulation in portions of Time.fixedDeltaTime
                // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
                while (timer >= 0)
                {
                    timer -= (float)ScriptTimerRef.Timer.UpdatedTimeInMilliseconds;
                    Physics2D.Simulate(PhysicsUpdateFrequency);
                }
            }

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

        private static string GetCollisionTriggerName(Object collider)
        {
            return $"Collider_{collider.GetInstanceID()}";
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

        public void UpdatePositions()
        {
            foreach (var obj in _physicsWorld)
            {
                obj.CalculatedSpeed = obj.Speed * FrameMultiplier * (float)ScriptTimerRef.Timer.UpdatedTimeInMilliseconds;

                obj.CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType.PreUpdate);
                obj.ImplyDirection(obj.CalculatedSpeed.normalized);

                //var speedResult = obj.CalculatedSpeed;
                //while(MaxDistanceBeforeCollisionCheck)

                obj.transform.position += (Vector3)obj.CalculatedSpeed;
                obj.CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType.PostUpdate);

                obj.CalculatedSpeed = obj.Speed;
            }

            foreach (var obj in _physicsStaticWorld)
            {
                obj.CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType.PreUpdate);
                obj.CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType.PostUpdate);
            }
        }

        public override object Value => this;

        public void Initialize()
        {
            Physics2D.autoSyncTransforms = true;
            _frameVariables = GetFrameVariables();
            _physicsWorld = new List<LichtPhysicsObject>();
            _physicsStaticWorld = new List<LichtPhysicsObject>();
            _physicsForces = new List<LichtPhysicsForce>();
            _customPhysicsForces = new Dictionary<string, LichtCustomPhysicsForce>();
            _semiSolids = new HashSet<Collider2D>();
            _colliderRegistry = new Dictionary<Collider2D, LichtPhysicsObject>();
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

        public void RegisterCollider(Collider2D col, LichtPhysicsObject obj)
        {
            _colliderRegistry[col] = obj;
        }

        public void UnregisterCollider(Collider2D col)
        {
            if (!_colliderRegistry.ContainsKey(col)) return;
            _colliderRegistry.Remove(col);
        }

        public bool TryGetPhysicsObjectByCollider(Collider2D col, out LichtPhysicsObject obj)
        {
            if (!_colliderRegistry.ContainsKey(col))
            {
                obj = null;
                return false;
            }

            obj = _colliderRegistry[col];
            return true;
        }
    }
}
