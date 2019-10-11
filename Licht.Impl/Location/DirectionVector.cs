﻿using Licht.Interfaces.Location;

namespace Licht.Impl.Numbers
{
    public class DirectionVector<TUnit, TAxis> : IDirectionVector<TUnit, TAxis>
    {
        public TAxis Axis { get; set; }
        public TUnit Value { get; set; }

        public DirectionVector(TAxis axis, TUnit unit)
        {
            Value = unit;
            Axis = axis;
        }
    }
}
