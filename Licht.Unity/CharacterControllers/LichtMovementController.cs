using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Licht.Unity.CharacterControllers
{
    public abstract class LichtMovementController : MonoBehaviour
    {
        private HashSet<MonoBehaviour> _movementBlockers;

        protected virtual void Awake()
        {
            _movementBlockers = new HashSet<MonoBehaviour>();
        }

        public void BlockMovement(MonoBehaviour source)
        {
            if (_movementBlockers.Contains(source)) return;
            _movementBlockers.Add(source);
        }

        public void UnblockMovement(MonoBehaviour source)
        {
            _movementBlockers.Remove(source);
        }

        public bool IsBlocked => _movementBlockers.Any();
    }
}