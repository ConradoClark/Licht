using Licht.Interfaces.Math;

namespace Licht.Impl.Numbers
{
    public class Point<T> : IQuantifiable<T>
    {
        public T Value { get; set; }

        public Point(T value)
        {
            Value = value;
        }

        public static implicit operator T(Point<T> t)
        {
            return t.Value;
        }

        public static implicit operator Point<T>(T t)
        {
            return new Point<T>(t);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
