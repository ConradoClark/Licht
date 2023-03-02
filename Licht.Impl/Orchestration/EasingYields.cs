using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;
using Licht.Impl.Globals;
using Licht.Interfaces.Time;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public static class EasingYields
    {
        public static IEnumerable<Action> Lerp(Action<float> setter,
            Func<float> getter, float seconds, float target, EasingFunction function, ITimer timer, Func<bool> breakCondition = null, bool setTargetOnBreak = false,
            float initStep = 0f, bool immediate = false, float? step = null, Func<bool> pauseCondition = null, Func<float,float> curve = null,
            Func<bool> resetCondition=null, Action<float> onEachStep = null, Action onStart = null)
        {
            return Lerp(setter, getter, seconds, () => target, function, timer, breakCondition, setTargetOnBreak,
                initStep, immediate, step, pauseCondition, curve, resetCondition, onEachStep, onStart);
        }

        public static IEnumerable<Action> Lerp(Action<float> setter,
            Func<float> getter, float seconds, Func<float> target, EasingFunction function, ITimer timer, Func<bool> breakCondition = null, bool setTargetOnBreak = false,
            float initStep = 0f, bool immediate = false, float? step = null, Func<bool> pauseCondition = null, Func<float, float> curve = null,
            Func<bool> resetCondition = null, Action<float> onEachStep = null, Action onStart = null)
        {
            if (!immediate) yield return TimeYields.WaitOneFrame;

            onStart?.Invoke();

            var ms = seconds * 1000d;
            var initialStart = getter();
            var start = initialStart;
            var last = start;

            if (ms > 0)
            {
                var time = initStep * ms;
                var prop = 1 / ms;
                while (time < ms)
                {
                    var initialTarget = target();

                    if (pauseCondition != null && pauseCondition())
                    {
                        yield return TimeYields.WaitOneFrame;
                        continue;
                    }

                    if (breakCondition != null && breakCondition())
                    {
                        if (setTargetOnBreak) setter(target());
                        yield break;
                    }

                    if (resetCondition != null && resetCondition())
                    {
                        time = initStep * ms;
                    }

                    time += timer.UpdatedTimeInMilliseconds;
                    var pos = getter();
                    var lastAcc = pos - last;
                    var lerpTarget = initialTarget + lastAcc;
                    start = initialStart + lastAcc;
                    
                    last = curve?.Invoke((float)(time * prop)) ?? Clamp(Interpolate((float)(time * prop), function), 0, 1);
                    last = Lerp(start, lerpTarget, last);
                    if (step != null)
                    {
                        last = GetStep(last, step.Value);
                    }

                    onEachStep?.Invoke(last);
                    setter(last);
                    yield return TimeYields.WaitOneFrame;
                }
            }

            var finalTarget = target();
            onEachStep?.Invoke(finalTarget);

            setter(finalTarget);
        }

        private static float Lerp(float p1, float p2, float fraction)
        {
            return p1 + (p2 - p1) * fraction;
        }

        public static float GetStep(float value, float step)
        {
            var absValue = Math.Abs(value);
            step = Math.Abs(step);

            var low = absValue - absValue % step;
            var high = low + step;

            var result = absValue - low < high - absValue ? low : high;
            return result * Math.Sign(value);
        }

        private static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }
        /// <summary>
        /// Constant Pi.
        /// </summary>
        private const float PI = (float)Math.PI;

        /// <summary>
        /// Constant Pi / 2.
        /// </summary>
        private const float HALFPI = (float)Math.PI / 2.0f;

        /// <summary>
        /// Easing Functions enumeration
        /// </summary>
        public enum EasingFunction
        {
            Linear,
            QuadraticEaseIn,
            QuadraticEaseOut,
            QuadraticEaseInOut,
            CubicEaseIn,
            CubicEaseOut,
            CubicEaseInOut,
            QuarticEaseIn,
            QuarticEaseOut,
            QuarticEaseInOut,
            QuinticEaseIn,
            QuinticEaseOut,
            QuinticEaseInOut,
            SineEaseIn,
            SineEaseOut,
            SineEaseInOut,
            CircularEaseIn,
            CircularEaseOut,
            CircularEaseInOut,
            ExponentialEaseIn,
            ExponentialEaseOut,
            ExponentialEaseInOut,
            ElasticEaseIn,
            ElasticEaseOut,
            ElasticEaseInOut,
            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,
            BounceEaseIn,
            BounceEaseOut,
            BounceEaseInOut
        }

        /// <summary>
        /// Interpolate using the specified function.
        /// </summary>
        private static float Interpolate(float p, EasingFunction function)
        {
            switch (function)
            {
                default:
                case EasingFunction.Linear: return Linear(p);
                case EasingFunction.QuadraticEaseOut: return QuadraticEaseOut(p);
                case EasingFunction.QuadraticEaseIn: return QuadraticEaseIn(p);
                case EasingFunction.QuadraticEaseInOut: return QuadraticEaseInOut(p);
                case EasingFunction.CubicEaseIn: return CubicEaseIn(p);
                case EasingFunction.CubicEaseOut: return CubicEaseOut(p);
                case EasingFunction.CubicEaseInOut: return CubicEaseInOut(p);
                case EasingFunction.QuarticEaseIn: return QuarticEaseIn(p);
                case EasingFunction.QuarticEaseOut: return QuarticEaseOut(p);
                case EasingFunction.QuarticEaseInOut: return QuarticEaseInOut(p);
                case EasingFunction.QuinticEaseIn: return QuinticEaseIn(p);
                case EasingFunction.QuinticEaseOut: return QuinticEaseOut(p);
                case EasingFunction.QuinticEaseInOut: return QuinticEaseInOut(p);
                case EasingFunction.SineEaseIn: return SineEaseIn(p);
                case EasingFunction.SineEaseOut: return SineEaseOut(p);
                case EasingFunction.SineEaseInOut: return SineEaseInOut(p);
                case EasingFunction.CircularEaseIn: return CircularEaseIn(p);
                case EasingFunction.CircularEaseOut: return CircularEaseOut(p);
                case EasingFunction.CircularEaseInOut: return CircularEaseInOut(p);
                case EasingFunction.ExponentialEaseIn: return ExponentialEaseIn(p);
                case EasingFunction.ExponentialEaseOut: return ExponentialEaseOut(p);
                case EasingFunction.ExponentialEaseInOut: return ExponentialEaseInOut(p);
                case EasingFunction.ElasticEaseIn: return ElasticEaseIn(p);
                case EasingFunction.ElasticEaseOut: return ElasticEaseOut(p);
                case EasingFunction.ElasticEaseInOut: return ElasticEaseInOut(p);
                case EasingFunction.BackEaseIn: return BackEaseIn(p);
                case EasingFunction.BackEaseOut: return BackEaseOut(p);
                case EasingFunction.BackEaseInOut: return BackEaseInOut(p);
                case EasingFunction.BounceEaseIn: return BounceEaseIn(p);
                case EasingFunction.BounceEaseOut: return BounceEaseOut(p);
                case EasingFunction.BounceEaseInOut: return BounceEaseInOut(p);
            }
        }

        /// <summary>
        /// Modeled after the line y = x
        /// </summary>
        private static float Linear(float p)
        {
            return p;
        }

        /// <summary>
        /// Modeled after the parabola y = x^2
        /// </summary>
        private static float QuadraticEaseIn(float p)
        {
            return p * p;
        }

        /// <summary>
        /// Modeled after the parabola y = -x^2 + 2x
        /// </summary>
        private static float QuadraticEaseOut(float p)
        {
            return p * (2 - p);
        }

        /// <summary>
        /// Modeled after the piecewise quadratic
        /// y = (1/2)((2x)^2)             ; [0, 0.5)
        /// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        /// </summary>
        private static float QuadraticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 2 * p * p;
            }

            return -2 * p * p + 4 * p - 1;
        }

        /// <summary>
        /// Modeled after the cubic y = x^3
        /// </summary>
        private static float CubicEaseIn(float p)
        {
            return p * p * p;
        }

        /// <summary>
        /// Modeled after the cubic y = (x - 1)^3 + 1
        /// </summary>
        private static float CubicEaseOut(float p)
        {
            var f = p - 1;
            return f * f * f + 1;
        }

        /// <summary>	
        /// Modeled after the piecewise cubic
        /// y = (1/2)((2x)^3)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        /// </summary>
        private static float CubicEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 4 * p * p * p;
            }

            var f = 2 * p - 2;
            return 0.5f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the quartic x^4
        /// </summary>
        private static float QuarticEaseIn(float p)
        {
            return p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quartic y = 1 - (x - 1)^4
        /// </summary>
        private static float QuarticEaseOut(float p)
        {
            var f = p - 1;
            return f * f * f * (1 - p) + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quartic
        /// y = (1/2)((2x)^4)        [0, 0.5]
        /// y = -(1/2)((2x-2)^4 - 2) [0.5, 1]
        /// </summary>
        private static float QuarticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 8 * p * p * p * p;
            }

            var f = p - 1;
            return -8 * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the quintic y = x^5
        /// </summary>
        private static float QuinticEaseIn(float p)
        {
            return p * p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quintic y = (x - 1)^5 + 1
        /// </summary>
        private static float QuinticEaseOut(float p)
        {
            var f = p - 1;
            return f * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quintic
        /// y = (1/2)((2x)^5)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        /// </summary>
        private static float QuinticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 16 * p * p * p * p * p;
            }

            var f = 2 * p - 2;
            return 0.5f * f * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave
        /// </summary>
        private static float SineEaseIn(float p)
        {
            return (float)Math.Sin((p - 1) * HALFPI) + 1;
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave (different phase)
        /// </summary>
        private static float SineEaseOut(float p)
        {
            return (float)Math.Sin(p * HALFPI);
        }

        /// <summary>
        /// Modeled after half sine wave
        /// </summary>
        private static float SineEaseInOut(float p)
        {
            return 0.5f * (1f - (float)Math.Cos(p * PI));
        }

        /// <summary>
        /// Modeled after shifted quadrant IV of unit circle
        /// </summary>
        private static float CircularEaseIn(float p)
        {
            return 1f - (float)Math.Sqrt(1 - p * p);
        }

        /// <summary>
        /// Modeled after shifted quadrant II of unit circle
        /// </summary>
        private static float CircularEaseOut(float p)
        {
            return (float)Math.Sqrt((2 - p) * p);
        }

        /// <summary>	
        /// Modeled after the piecewise circular function
        /// y = (1/2)(1 - Math.Sqrt(1 - 4x^2))           ; [0, 0.5)
        /// y = (1/2)(Math.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        /// </summary>
        private static float CircularEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 0.5f * (1 - (float)Math.Sqrt(1 - 4 * (p * p)));
            }

            return 0.5f * ((float)Math.Sqrt(-(2 * p - 3) * (2 * p - 1)) + 1);
        }

        /// <summary>
        /// Modeled after the exponential function y = 2^(10(x - 1))
        /// </summary>
        private static float ExponentialEaseIn(float p)
        {
            return Math.Abs(p) < NumericGlobals.FloatTolerance ? p : (float)Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the exponential function y = -2^(-10x) + 1
        /// </summary>
        private static float ExponentialEaseOut(float p)
        {
            return Math.Abs(p - 1.0f) < NumericGlobals.FloatTolerance ? p : 1 - (float)Math.Pow(2, -10 * p);
        }

        /// <summary>
        /// Modeled after the piecewise exponential
        /// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        /// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        /// </summary>
        private static float ExponentialEaseInOut(float p)
        {
            if (Math.Abs(p) < NumericGlobals.FloatTolerance || Math.Abs(p - 1.0) < NumericGlobals.FloatTolerance) return p;

            if (p < 0.5f)
            {
                return 0.5f * (float)Math.Pow(2, 20 * p - 10);
            }

            return -0.5f * (float)Math.Pow(2, -20 * p + 10) + 1;
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(13pi/2*x)*Math.Pow(2, 10 * (x - 1))
        /// </summary>
        private static float ElasticEaseIn(float p)
        {
            return (float)Math.Sin(13 * HALFPI * p) * (float)Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*Math.Pow(2, -10x) + 1
        /// </summary>
        private static float ElasticEaseOut(float p)
        {
            return (float)Math.Sin(-13 * HALFPI * (p + 1)) * (float)Math.Pow(2, -10 * p) + 1;
        }

        /// <summary>
        /// Modeled after the piecewise exponentially-damped sine wave:
        /// y = (1/2)*sin(13pi/2*(2*x))*Math.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        /// y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        /// </summary>
        private static float ElasticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 0.5f * (float)Math.Sin(13 * HALFPI * (2 * p)) * (float)Math.Pow(2, 10 * (2 * p - 1));
            }

            return 0.5f * ((float)Math.Sin(-13 * HALFPI * (2 * p - 1 + 1)) * (float)Math.Pow(2, -10 * (2 * p - 1)) + 2);
        }

        /// <summary>
        /// Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        /// </summary>
        private static float BackEaseIn(float p)
        {
            return p * p * p - p * (float)Math.Sin(p * PI);
        }

        /// <summary>
        /// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        /// </summary>	
        private static float BackEaseOut(float p)
        {
            var f = 1 - p;
            return 1f - (f * f * f - f * (float)Math.Sin(f * PI));
        }

        /// <summary>
        /// Modeled after the piecewise overshooting cubic function:
        /// y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        /// y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        /// </summary>
        private static float BackEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                var f = 2 * p;
                return 0.5f * (f * f * f - f * (float)Math.Sin(f * PI));
            }
            else
            {
                var f = 1 - (2 * p - 1);
                return 0.5f * (1 - (f * f * f - f * (float)Math.Sin(f * PI))) + 0.5f;
            }
        }

        /// <summary>
        /// </summary>
        private static float BounceEaseIn(float p)
        {
            return 1 - BounceEaseOut(1 - p);
        }

        /// <summary>
        /// </summary>
        private static float BounceEaseOut(float p)
        {
            if (p < 4 / 11.0f)
            {
                return 121 * p * p / 16.0f;
            }

            if (p < 8 / 11.0f)
            {
                return 363 / 40.0f * p * p - 99 / 10.0f * p + 17 / 5.0f;
            }

            if (p < 9 / 10.0f)
            {
                return 4356 / 361.0f * p * p - 35442 / 1805.0f * p + 16061 / 1805.0f;
            }

            return 54 / 5.0f * p * p - 513 / 25.0f * p + 268 / 25.0f;
        }

        /// <summary>
        /// </summary>
        private static float BounceEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 0.5f * BounceEaseIn(p * 2);
            }

            return 0.5f * BounceEaseOut(p * 2 - 1) + 0.5f;
        }
    }
}
