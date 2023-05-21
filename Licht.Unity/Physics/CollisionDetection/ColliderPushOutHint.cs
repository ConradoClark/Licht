using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Licht.Unity.Objects;
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

        [field:SerializeField]
        public PushOutDirection HorizontalHintDirection { get; set; }


        [field: SerializeField]
        public PushOutDirection VerticalHintDirection { get; set; }
    }
}
