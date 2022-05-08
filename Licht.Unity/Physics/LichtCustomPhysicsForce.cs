using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using UnityEngine;

namespace Licht.Unity.Physics
{
    public abstract class LichtCustomPhysicsForce : MonoBehaviour, IActivable, IDeactivable
    {
        public abstract bool IsActive { get; set; }
        public abstract bool Deactivate();
        public abstract bool Activate();
        public abstract bool ActivateFor(LichtPhysicsObject physicsObject);
        public abstract bool DeactivateFor(LichtPhysicsObject physicsObject);
        public abstract string Key { get; }

        protected Dictionary<LichtPhysicsObject, bool> ActivationFlags;
        protected LichtPhysics Physics;

        protected Dictionary<LichtPhysicsObject, HashSet<MonoBehaviour>> ForceBlockers;

        public void BlockForceFor(MonoBehaviour source, LichtPhysicsObject target)
        {
            if (!ForceBlockers.ContainsKey(target))
            {
                ForceBlockers[target] = new HashSet<MonoBehaviour>();
            }
        
            if (!ForceBlockers[target].Contains(source)) ForceBlockers[target].Add(source);

        }

        public void UnblockForceFor(MonoBehaviour source, LichtPhysicsObject target)
        {
            if (!ForceBlockers.ContainsKey(target)) return;
            if (ForceBlockers[target].Contains(source)) ForceBlockers[target].Remove(source);
        }

        public bool IsBlocked(LichtPhysicsObject obj) => ForceBlockers.ContainsKey(obj) && ForceBlockers[obj].Any();

        protected virtual void Awake()
        {
            Physics = this.GetLichtPhysics();
            ActivationFlags = new Dictionary<LichtPhysicsObject, bool>();
            ForceBlockers = new Dictionary<LichtPhysicsObject, HashSet<MonoBehaviour>>();
        }

        protected virtual void OnEnable()
        {
            Physics.AddPhysicsForce(this);
        }

        protected virtual void OnDisable()
        {
            Physics.RemovePhysicsForce(this);
            Deactivate();
        }
    }
}