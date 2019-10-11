using Licht.Interfaces.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Location
{
    public interface IDirectionVector<out TLocationUnit, out TAxis> : IQuantifiable<TLocationUnit>
    {
        TAxis Axis { get; }
    }
}
