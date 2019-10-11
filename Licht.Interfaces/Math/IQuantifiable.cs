namespace Licht.Interfaces.Math
{
    public interface IQuantifiable<out TUnit>
    {
        TUnit Value { get; }
    }
}
