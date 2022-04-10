using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;

namespace Licht.Unity.Accessors
{
    public class AxisAccessor
    {
        private readonly TransformAccessor _accessor;
        public AxisAccessor(TransformAccessor accessor)
        {
            _accessor = accessor;
        }

        public LerpBuilder X =>
            new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.X),
                _accessor.Getter(TransformExtensions.Axis.X));
        public LerpBuilder Y =>
            new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.Y),
                _accessor.Getter(TransformExtensions.Axis.Y));
        public LerpBuilder Z =>
            new LerpBuilder(_accessor.Setter(TransformExtensions.Axis.Z),
                _accessor.Getter(TransformExtensions.Axis.Z));
    }
}
