using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Math
{
    public interface INumeric<T>: IEquatable<T> where T: INumeric<T>
    {
        T FromDouble(double x);
        T FromInt(int x);
        T Add(T x);
        T Subtract(T x);
        T Multiply(T x);
        T Divide(T x);
        T Abs();
        T Acos();
        T Atan();
        T Atan2(T x);
        T ASin();
        T Ceiling();
        T Cos();
        T Cosh();
        T Exp();
        T Floor();
        T Log();
        T Log10();
        T Log(T x);
        T Max(T x);
        T Min(T x);
        T Pow(T x);
        T Round();
        T Sign();
        T Sin();
        T Sinh();
        T Sqrt();
        T Tan();
        T Tanh();
        T Truncate();
        T One();
        T Zero();
        T ChangeSign();
        void Set(T value);
        T Get();
        bool LessThan(T other);
        bool LessOrEqualThan(T other);
        new bool Equals(T other);
        bool GreaterThan(T other);
        bool GreaterOrEqualThan(T other);
    }
}
