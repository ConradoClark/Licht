using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Builders;
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

        public CustomAxisAccessor LocalScale => new CustomAxisAccessor(
            value => _transform.localScale = new Vector3(value, _transform.localScale.y, _transform.localScale.z),
            value => _transform.localScale = new Vector3(_transform.localScale.x, value, _transform.localScale.z),
            value => _transform.localScale = new Vector3(_transform.localScale.x, _transform.localScale.y, value),
            () => _transform.localScale.x, () => _transform.localScale.y, () => _transform.localScale.z);

        public LerpBuilder UniformScale(bool includeZAxis=false) => new LerpBuilder(
            value => _transform.localScale = new Vector3(value, value, includeZAxis ? value : _transform.localScale.z),
            () => _transform.localScale.x);

    }
}
