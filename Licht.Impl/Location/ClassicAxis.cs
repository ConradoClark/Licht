using System;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Location;

namespace Licht.Impl.Numbers
{
    [PublicAPI]
    public class ClassicAxis : IEquatable<ClassicAxis>, IAxis
    {
        public char AxisLetter { get; protected set; }
        public ClassicAxis(char axisLetter)
        {
            AxisLetter = axisLetter;
        }

        // Basic Presets
        public static readonly ClassicAxis X = new ClassicAxis(nameof(X).Single());
        public static readonly ClassicAxis Y = new ClassicAxis(nameof(Y).Single());
        public static readonly ClassicAxis Z = new ClassicAxis(nameof(Z).Single());

        public static implicit operator ClassicAxis(char axisLetter)
        {
            return new ClassicAxis(axisLetter);
        }

        public bool Equals(ClassicAxis other)
        {
            if (other == null) return false;
            return AxisLetter == other.AxisLetter;
        }
    }
}
