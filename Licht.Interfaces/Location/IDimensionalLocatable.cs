using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Location
{
    public interface IDimensionalLocatable<TLocationUnit, in TAxis>
    {
        TLocationUnit this[TAxis index]
        {
            get;
            set;
        }
    }
}
