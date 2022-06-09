using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Licht.Unity.CharacterControllers
{
    public abstract class LichtMovementController : MonoBehaviour
    {
        private HashSet<Object> _movementBlockers;

        protected virtual void Awake()
        {
            _movementBlockers = new HashSet<Object>();
        }

        public void BlockMovement(Object source)
        {
            if (_movementBlockers.Contains(source)) return;
            _movementBlockers.Add(source);
        }

        public void UnblockMovement(Object source)
        {
            _movementBlockers.Remove(source);
        }

        public bool IsBlocked => _movementBlockers.Any();
    }
}