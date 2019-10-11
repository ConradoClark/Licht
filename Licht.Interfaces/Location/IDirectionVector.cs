using Licht.Interfaces.Math;

namespace Licht.Interfaces.Location
{
    public interface IDirectionVector<out TLocationUnit, out TAxis> : IQuantifiable<TLocationUnit>
    {
        TAxis Axis { get; }
    }
}
