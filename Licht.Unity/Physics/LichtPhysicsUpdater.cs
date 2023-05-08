using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics
{
    [CreateAssetMenu(fileName = "LichtPhysicsUpdater", menuName = "Licht/Physics/LichtPhysicsUpdater", order = 1)]
    public class LichtPhysicsUpdater : ScriptValue, ICanInitialize, IUpdateable
    {
        private FrameVariables _frameVariables;
        private LichtPhysics _physics;
        public bool IsUpdating { get; private set; }

        public FrameVariables GetFrameVariables()
        {
            if (_frameVariables != null) return _frameVariables;
            _frameVariables = FindObjectOfType<FrameVariables>();

            if (_frameVariables != null) return _frameVariables;
            var obj = new GameObject("frameVars");
            return _frameVariables = obj.AddComponent<FrameVariables>();
        }

        public void Update()
        {
            _physics.UpdatePositions();
            if (_physics.Debug)
            {
                IsUpdating = true;
            }
        }

        public override object Value => this;

        public void Initialize()
        {
            _frameVariables = GetFrameVariables();
            _physics = this.GetLichtPhysics();
        }
    }
}
