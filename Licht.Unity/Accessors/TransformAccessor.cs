using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Extensions;
using UnityEngine;

namespace Licht.Unity.Accessors
{
    public class TransformAccessor
    {
        private readonly Transform _transform;
        public TransformAccessor(Transform transform)
        {
            _transform = transform;
        }

        public Action<float> Setter(TransformExtensions.Axis axis)
        {
            return _transform.PositionSetter(axis);
        }

        public Func<float> Getter(TransformExtensions.Axis axis)
        {
            return _transform.PositionGetter(axis);
        }

        public AxisAccessor Position => new AxisAccessor(this);
    }
}
