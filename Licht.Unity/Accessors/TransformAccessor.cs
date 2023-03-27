using System;
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

        public Action<float> LocalSetter(TransformExtensions.Axis axis)
        {
            return _transform.LocalPositionSetter(axis);
        }

        public Func<float> LocalGetter(TransformExtensions.Axis axis)
        {
            return _transform.LocalPositionGetter(axis);
        }


        public AxisAccessor Position => new AxisAccessor(this);

        public AxisAccessor LocalPosition => new AxisAccessor(this, true);

        public CustomAxisAccessor LocalScale => new CustomAxisAccessor(
            value => _transform.localScale = new Vector3(value, _transform.localScale.y, _transform.localScale.z),
            value => _transform.localScale = new Vector3(_transform.localScale.x, value, _transform.localScale.z),
            value => _transform.localScale = new Vector3(_transform.localScale.x, _transform.localScale.y, value),
            () => _transform.localScale.x, () => _transform.localScale.y, () => _transform.localScale.z);

        public LerpBuilder Rotation(TransformExtensions.Axis axis)
        {
            var eulerRotation = _transform.localEulerAngles;
            return new LerpBuilder(
                value => _transform.localRotation = Quaternion.Euler(
                    axis == TransformExtensions.Axis.X ? value : eulerRotation.x,
                        axis == TransformExtensions.Axis.Y ? value : eulerRotation.y,
                        axis == TransformExtensions.Axis.Z ? value : eulerRotation.z),
                    () => axis switch
                    {
                        TransformExtensions.Axis.X => ConvertEulerAngles(_transform.localEulerAngles.x),
                        TransformExtensions.Axis.Y => ConvertEulerAngles(_transform.localEulerAngles.y),
                        TransformExtensions.Axis.Z => ConvertEulerAngles(_transform.localEulerAngles.z),
                        _ => 0
                    });
        }

        private float ConvertEulerAngles(float angle)
        {
            return angle > 180 ? angle - 360 : angle;
        }

        public LerpBuilder UniformScale(bool includeZAxis = false) => new LerpBuilder(
            value => _transform.localScale = new Vector3(value, value, includeZAxis ? value : _transform.localScale.z),
            () => _transform.localScale.x);

    }
}
