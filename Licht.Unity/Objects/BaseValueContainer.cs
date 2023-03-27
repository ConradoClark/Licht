using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Licht.Unity.Objects
{
    public class BaseValueContainerManager<T>
    {
        public static Dictionary<object, BaseValueContainer<T>> ObjectContainer;
        static BaseValueContainerManager()
        {
            ObjectContainer = new Dictionary<object, BaseValueContainer<T>>();
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ObjectContainer.Clear();
        }

        public static BaseValueContainer<T> CreateOrGetContainer(object source)
        {
            if (!ObjectContainer.ContainsKey(source))
            {
                ObjectContainer[source] = new BaseValueContainer<T>();
            }
            return ObjectContainer[source];
        }
    }

    public struct ValueContainerChangeEvent<T>
    {
        public string Property { get; set; }
        public T NewValue { get; set; }
        public T OldValue { get; set; }
    }

    public class BaseValueContainer<T>
    {
        protected virtual Dictionary<string, T> Props { get; set; } = new Dictionary<string, T>();
        public event Action<ValueContainerChangeEvent<T>> OnValueChanged;

        public T this[string prop]
        {
            get => Props[prop];
            set
            {
                var oldValue = Props.ContainsKey(prop) ? Props[prop] : default;
                Props[prop] = value;
                OnValueChanged?.Invoke(new ValueContainerChangeEvent<T>
                {
                    Property = prop,
                    OldValue = oldValue,
                    NewValue= value
                });
            }
        }

        public virtual bool HasProp(string prop, T value = default)
        {
            return Props.ContainsKey(prop) &&
                   (value == null || value.Equals(default(T)) || Props[prop].Equals(value));
        }

        public virtual bool TryGetProp(string prop, out T value)
        {
            value = default;
            if (!Props.ContainsKey(prop)) return false;

            value = Props[prop];
            return true;
        }

        public virtual void ResetProps()
        {
            Props.Clear();
        }
    }

    public static class BaseValueContainerExtensions
    {
        public static BaseValueContainer<T> Props<T>(this MonoBehaviour source)
        {
            return BaseValueContainerManager<T>.CreateOrGetContainer(source);
        }

        public static BaseValueContainer<T> Props<T>(this GameObject source)
        {
            return BaseValueContainerManager<T>.CreateOrGetContainer(source);
        }
    }
}
