using Licht.Unity.Builders;
using Licht.Unity.Extensions;

namespace Licht.Unity.Accessors
{
    public class AxisAccessor
    {
        private readonly TransformAccessor _accessor;
        private readonly bool _useLocal;
        public AxisAccessor(TransformAccessor accessor, bool useLocal = false)
        {
            _accessor = accessor;
            _useLocal = useLocal;
        }

        public LerpBuilder X =>
            _useLocal ? new LerpBuilder(_accessor.LocalSetter(TransformExtensions.Axis.X),
                _accessor.LocalGetter(TransformExtensions.Axis.X)) :
            new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.X),
                _accessor.Getter(TransformExtensions.Axis.X));
        public LerpBuilder Y =>
            _useLocal ? new LerpBuilder(_accessor.LocalSetter(TransformExtensions.Axis.Y),
                    _accessor.LocalGetter(TransformExtensions.Axis.Y)) :
                new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.Y),
                    _accessor.Getter(TransformExtensions.Axis.Y));
        public LerpBuilder Z =>
            _useLocal ? new LerpBuilder(_accessor.LocalSetter(TransformExtensions.Axis.Z),
                    _accessor.LocalGetter(TransformExtensions.Axis.Z)) :
                new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.Z),
                    _accessor.Getter(TransformExtensions.Axis.Z));
    }
}
