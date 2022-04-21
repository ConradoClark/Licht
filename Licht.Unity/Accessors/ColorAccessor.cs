using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Builders;
using UnityEngine;

namespace Licht.Unity.Accessors
{
    public class ColorAccessor
    {
        private readonly Action<Color> _setter;
        private readonly Func<Color> _getter;

        public ColorAccessor(Action<Color> setter, Func<Color> getter)
        {
            _setter = setter;
            _getter = getter;
        }

        public LerpBuilder ToColor(Color color)
        {
            var current = _getter();
            var @ref = 0f;
                return new LerpBuilder(value =>
                    {
                        _setter(Color.Lerp(current, color, value));
                        @ref = value;
                    },
                    () => @ref);
        }

        public LerpBuilder R
        {
            get
            {
                var current = _getter();
                return new LerpBuilder(value => _setter(new Color(value, current.g, current.b, current.a)),
                    () => _getter().r);
            }
        }

        public LerpBuilder G
        {
            get
            {
                var current = _getter();
                return new LerpBuilder(value => _setter(new Color(current.r, value, current.b, current.a)),
                    () => _getter().g);
            }
        }

        public LerpBuilder B
        {
            get
            {
                var current = _getter();
                return new LerpBuilder(value => _setter(new Color(current.r, current.g, value, current.a)),
                    () => _getter().b);
            }
        }

        public LerpBuilder A
        {
            get
            {
                var current = _getter();
                return new LerpBuilder(value => _setter(new Color(current.r, current.g, current.b, value)),
                    () => _getter().a);
            }
        }
    }
}
