using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Memory;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class CollisionPositionClampRegister
    {
        public Collider2D Origin { get; set; }
        public Collider2D Target { get; set; }
        public Vector2 Clamp { get; set; }
    }
}
