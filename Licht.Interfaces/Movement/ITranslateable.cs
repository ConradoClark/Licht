using Licht.Interfaces.Location;
using Licht.Interfaces.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Movement
{
    public interface ITranslateable<in TLocationUnit, in TAxis>
    {
        void Translate(TLocationUnit amount, params IDirectionVector<TLocationUnit, TAxis>[] directionsVector);
        void Set(params IDirectionVector<TLocationUnit, TAxis>[] directionsVector);
    }
}
