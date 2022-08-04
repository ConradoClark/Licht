using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.CharacterControllers
{
    public abstract class LichtMovementController : BaseGameObject
    {
        public struct MovementMultiplier
        {
            public Object Source;
            public Vector2[] Multipliers;
        }

        private HashSet<Object> _movementBlockers;
        private HashSet<MovementMultiplier> _movementMultipliers;

        protected override void OnAwake()
        {
            base.OnAwake();
            _movementBlockers = new HashSet<Object>();
            _movementMultipliers = new HashSet<MovementMultiplier>();
        }

        public void SetMovementMultipliers(Object source, params Vector2[] multipliers)
        {
            var moveMultiplier = new MovementMultiplier
            {
                Source = source,
                Multipliers = multipliers
            };

            if (_movementMultipliers.Contains(moveMultiplier)) return;
            _movementMultipliers.Add(moveMultiplier);
        }

        public void RemoveMovementMultipliers(Object source, params Vector2[] multipliers)
        {
            var moveMultiplier = new MovementMultiplier
            {
                Source = source,
                Multipliers = multipliers
            };

            if (!_movementMultipliers.Contains(moveMultiplier)) return;
            _movementMultipliers.Remove(moveMultiplier);
        }

        protected float GetMovementMultiplier(float speed, Vector2 direction)
        {
            var multiplier = _movementMultipliers.SelectMany(m => m.Multipliers)
                .Where(m => m.normalized == direction.normalized)
                .Aggregate(1f, (f, v) => f * v.magnitude);

            return speed * multiplier;
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