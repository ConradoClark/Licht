﻿using Licht.Interfaces.Location;

namespace Licht.Interfaces.Movement
{
    public interface ITranslateable<in TLocationUnit, in TAxis>
    {
        void Translate(TLocationUnit amount, params IDirectionVector<TLocationUnit, TAxis>[] directionsVector);
        void Set(params IDirectionVector<TLocationUnit, TAxis>[] directionsVector);
    }
}
