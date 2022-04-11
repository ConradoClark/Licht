using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;

namespace Licht.Unity.Accessors
{
    public class CustomAxisAccessor
    {
        private readonly Action<float> _setterX;
        private readonly Action<float> _setterY;
        private readonly Action<float> _setterZ;
        private readonly Func<float> _getterX;
        private readonly Func<float> _getterY;
        private readonly Func<float> _getterZ;

        public CustomAxisAccessor(Action<float> setterX,
            Action<float> setterY,
            Action<float> setterZ,
            Func<float> getterX,
            Func<float> getterY,
            Func<float> getterZ)
        {
            _setterX = setterX;
            _setterY = setterY;
            _setterZ = setterZ;
            _getterX = getterX;
            _getterY = getterY;
            _getterZ = getterZ;
        }

        public LerpBuilder X =>
            new LerpBuilder(value => _setterX(value), () => _getterX());
        public LerpBuilder Y =>
            new LerpBuilder(value => _setterY(value), () => _getterY());
        public LerpBuilder Z =>
            new LerpBuilder(value => _setterZ(value), () => _getterZ());
    }
}
