using Licht.Unity.Builders;
using Licht.Unity.Physics;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class LichtPhysicsObjectExtensions
    {
        public static PhysicsSpeedAccessor GetSpeedAccessor(this LichtPhysicsObject obj, Vector2? initialSpeed = null)
        {
            return new PhysicsSpeedAccessor(obj, initialSpeed ?? Vector2.zero);
        }

        public class PhysicsSpeedAccessor
        {
            private readonly LichtPhysicsObject _obj;
            private readonly Vector2 _initialSpeed;

            public PhysicsSpeedAccessor(LichtPhysicsObject obj, Vector2 initialSpeed)
            {
                _obj = obj;
                _initialSpeed = initialSpeed;
            }

            public LerpBuilder X
            {
                get
                {
                    var refSpeed = _initialSpeed.x;
                    return new LerpBuilder(
                        f =>
                        {
                            refSpeed = f;
                            _obj.ApplySpeed(new Vector2(refSpeed,0));
                        }, () => refSpeed);
                }
            }

            public LerpBuilder Y
            {
                get
                {
                    var refSpeed = _initialSpeed.y;
                    return new LerpBuilder(
                        f =>
                        {
                            refSpeed = f;
                            _obj.ApplySpeed(new Vector2(0, refSpeed));
                        }, () => refSpeed);
                }
            }
        }
    }
}
