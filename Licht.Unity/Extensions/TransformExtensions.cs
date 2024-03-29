﻿using System;
using Licht.Unity.Accessors;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class TransformExtensions
    {
        public enum Axis
        {
            X, Y, Z
        }
        public static Action<float> PositionSetter(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return (value) => transform.position = new Vector3(value, transform.position.y, transform.position.z);
                case Axis.Y:
                    return (value) => transform.position = new Vector3(transform.position.x, value, transform.position.z);
                case Axis.Z:
                    return (value) => transform.position = new Vector3(transform.position.x, transform.position.y, value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static Func<float> PositionGetter(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return () => transform.position.x;
                case Axis.Y:
                    return () => transform.position.y;
                case Axis.Z:
                    return () => transform.position.z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static Action<float> LocalPositionSetter(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return (value) => transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
                case Axis.Y:
                    return (value) => transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
                case Axis.Z:
                    return (value) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static Func<float> LocalPositionGetter(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return () => transform.localPosition.x;
                case Axis.Y:
                    return () => transform.localPosition.y;
                case Axis.Z:
                    return () => transform.localPosition.z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static TransformAccessor GetAccessor(this Transform transform)
        {
            return new TransformAccessor(transform);
        }
    }
}
