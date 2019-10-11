using System;
using System.Linq;
using Licht.Interfaces.Location;
using System.Collections.Generic;
using Licht.Impl.Globals;
using Licht.Interfaces.Movement;
using Licht.Interfaces.Math;

namespace Licht.Impl.Numbers
{
    public struct GenericVector<TAxis> : IDimensionalLocatable<float, TAxis>, ITranslateable<float, TAxis>
        where TAxis : IAxis
    {
        private readonly IReadOnlyList<TAxis> _validAxis;
        private readonly Dictionary<TAxis, Point<float>> _vector;

        public GenericVector(params TAxis[] axis)
        {
            _vector = axis.Distinct()
                .ToDictionary(a => a, a => new Point<float>(default(float)));

            _validAxis = axis.ToList();
        }

        public float this[TAxis index]
        {
            get => IsAxisValid(index) ? _vector[index].Value : default(float);
            set
            {
                if (!IsAxisValid(index)) return;
                _vector[index].Value = value;
            }
        }

        public void Translate(float amount, params IDirectionVector<float, TAxis>[] directionsVector)
        {
            var t = this;
            var ls = directionsVector.Where(d => t.IsAxisValid(d.Axis))
                             .ToLookup(d => d.Axis, d => d.Value)
                             .Select(d => new DirectionVector<float, TAxis>(d.Key, d.Aggregate((d1, d2) => d1+d2)))
                             .Select(d => d.Value * d.Value)
                             .Aggregate((d1, d2) => d1+d2);

            var length = (float) Math.Sqrt(ls);
            foreach (var dir in directionsVector.Where(d => t.IsAxisValid(d.Axis)))
            {
                var unit = _vector[dir.Axis].Value;
                _vector[dir.Axis].Value = length.FloatEq(0) ? unit : unit + (dir.Value / length * amount);
            }
        }

        public void Set(params IDirectionVector<float, TAxis>[] directionsVector)
        {
            var t = this;
            foreach (var dir in directionsVector.Where(d => t.IsAxisValid(d.Axis)))
            {
                _vector[dir.Axis] = dir.Value;
            }
        }

        public bool IsAxisValid(TAxis axis)
        {
            return _validAxis.Any(c => c.AxisLetter == axis.AxisLetter);
        }

        public override string ToString()
        {
            return $"({string.Join(",", _vector.Values)})";
        }

        public static GenericVector<TAxis> operator +(GenericVector<TAxis> v1, GenericVector<TAxis> v2)
        {
            var axis = v1._validAxis.Union(v2._validAxis).ToArray();

            var result = new GenericVector<TAxis>(axis);
            foreach (TAxis ax in axis)
            {
                var v1Valid = v1.IsAxisValid(ax);
                var v2Valid = v2.IsAxisValid(ax);

                if (v1Valid && v2Valid)
                {
                    result[ax] = v1[ax] + v2[ax];
                    continue;
                }
                if (v1Valid)
                {
                    result[ax] = v1[ax];
                    continue;
                }
                if (v2Valid)
                {
                    result[ax] = v2[ax];
                }
            }
            return result;
        }

        public static GenericVector<TAxis> operator -(GenericVector<TAxis> v1, GenericVector<TAxis> v2)
        {
            var axis = v1._validAxis.Union(v2._validAxis).ToArray();

            var result = new GenericVector<TAxis>(axis);
            foreach (TAxis ax in axis)
            {
                var v1Valid = v1.IsAxisValid(ax);
                var v2Valid = v2.IsAxisValid(ax);

                if (v1Valid && v2Valid)
                {
                    result[ax] = v1[ax] - v2[ax];
                    continue;
                }
                if (v1Valid)
                {
                    result[ax] = v1[ax];
                    continue;
                }
                if (v2Valid)
                {
                    result[ax] = -v2[ax];
                }
            }
            return result;
        }
    }
}