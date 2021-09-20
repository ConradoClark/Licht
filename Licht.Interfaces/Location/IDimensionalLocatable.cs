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
