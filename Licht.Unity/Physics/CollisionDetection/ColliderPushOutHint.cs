using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class ColliderPushOutHint : BaseGameObject
    {
        [Serializable]
        public enum PushOutDirection
        {
            None,
            Positive,
            Negative,
        }

        [field: CustomLabel("Where should the collider push objects out")]
        [field: CustomLabel("Positive: Right. Negative: Left")]
        [field: SerializeField]
        public PushOutDirection HorizontalHintDirection { get; set; }

        [field: CustomLabel("Where should the collider push objects out")]
        [field: CustomLabel("Positive: Up. Negative: Down")]
        [field: SerializeField]
        public PushOutDirection VerticalHintDirection { get; set; }
    }
}
