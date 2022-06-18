using Licht.Unity.Builders;
using UnityEngine;

namespace Licht.Unity.Accessors
{
    public class MaterialAccessor
    {
        private readonly string _property;
        private readonly SpriteRenderer _renderer;
        public MaterialAccessor(string property, SpriteRenderer renderer)
        {
            _property = property;
            _renderer = renderer;
        }

        public LerpBuilder AsFloat()
        {
            return new LerpBuilder(f => _renderer.material.SetFloat(_property, f),
                () => _renderer.material.GetFloat(_property));
        }

        public ColorAccessor AsColor()
        {
            return new ColorAccessor(c => _renderer.material.SetColor(_property, c),
                () => _renderer.material.GetColor(_property));
        }

        public LerpBuilder AsInt()
        {
            return new LerpBuilder(f => _renderer.material.SetInt(_property, Mathf.RoundToInt(f)),
                () => _renderer.material.GetInt(_property));
        }
    }
}
