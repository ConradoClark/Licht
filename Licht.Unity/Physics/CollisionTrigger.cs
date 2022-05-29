using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Physics
{
    public struct CollisionTrigger
    {
        public LichtPhysicsObject Actor;
        public Collider2D Target;

        public override bool Equals(object obj)
        {
            if (!(obj is CollisionTrigger trigger)) return false;
            return Actor == trigger.Actor && Target == trigger.Target;
        }

        public override int GetHashCode()
        {
            return Actor?.GetHashCode() ?? 0 ^ Target?.GetHashCode() ?? 0;
        }
    }
}
