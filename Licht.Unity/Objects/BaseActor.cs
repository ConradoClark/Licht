using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Extensions;

namespace Licht.Unity.Objects
{
    public class BaseActor : BaseGameObject
    {
        private Dictionary<Type, object> _customObjects;

        public void AddCustomObject<T>(T obj) where T : class
        {
            _customObjects ??= new Dictionary<Type, object>();
            _customObjects[typeof(T)] = obj;
        }

        public void AddCustomObject(Type type, BaseGameObject obj)
        {
            _customObjects ??= new Dictionary<Type, object>();
            _customObjects[type] = obj;
        }

        public void RemoveCustomObject<T>() where T : class
        {
            _customObjects ??= new Dictionary<Type, object>();
            if (!_customObjects.ContainsKey(typeof(T))) return;

            _customObjects.Remove(typeof(T));
        }

        public bool TryGetCustomObject<T>(out T obj) where T : class
        {
            _customObjects ??= new Dictionary<Type, object>();
            if (!_customObjects.ContainsKey(typeof(T)))
            {
                obj = default;
                return false;
            }

            obj = _customObjects[typeof(T)] as T;
            return true;
        }

        public bool HasCustomObjectOfType(Type type)
        {
            _customObjects ??= new Dictionary<Type, object>();
            return _customObjects.ContainsKey(type);
        }
    }
}
