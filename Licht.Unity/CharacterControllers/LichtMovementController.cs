using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.CharacterControllers
{
    public abstract class LichtMovementController : BaseGameObject
    {
        private HashSet<Object> _movementBlockers;

        protected override void OnAwake()
        {
            base.OnAwake();
            _movementBlockers = new HashSet<Object>();
        }

        public void BlockMovement(Object source)
        {
            if (_movementBlockers.Contains(source)) return;
            _movementBlockers.Add(source);
        }

        public void UnblockMovement(Object source)
        {
            if (!_movementBlockers.Contains(source)) return;
            _movementBlockers.Remove(source);
        }

        public bool IsBlocked => _movementBlockers.Any();
    }
}