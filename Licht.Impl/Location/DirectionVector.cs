using Licht.Interfaces.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
