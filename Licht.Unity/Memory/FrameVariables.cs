using System;
using System.Collections.Generic;
using UnityEngine;

namespace Licht.Unity.Memory
{
    public class FrameVariables : MonoBehaviour
    {

        internal static class FrameVariablesStatic<T>
        {
            private static readonly Dictionary<string, T> Variables = new Dictionary<string, T>();

            public static (T Variable, Action ClearAction) Get(string variable, Func<T> getter)
            {
                if (Variables.ContainsKey(variable)) return (Variables[variable], Clear);
                return (Variables[variable] = getter(), Clear);
            }

            public static Action Set(string variable, T value)
            {
                Variables[variable] = value;
                return Clear;
            }

            public static void Clear()
            {
                Variables.Clear();
            }
        }

        private readonly Dictionary<Type, Action> _clearActions = new Dictionary<Type, Action>();

        private void Update()
        {
            Clear();
        }

        private void OnDisable()
        {
            Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (var action in _clearActions.Values)
            {
                action();
            }
            _clearActions.Clear(); ;
        }

        public T Get<T>(FrameVariableDefinition<T> definition)
        {
            var (result, clearAction) = FrameVariablesStatic<T>.Get(definition.Variable, definition.Getter);
            _clearActions[typeof(T)] = clearAction;
            return result;
        }

        public void Set<T>(string variableName, T value)
        {
            var clearAction = FrameVariablesStatic<T>.Set(variableName, value);
            _clearActions[typeof(T)] = clearAction;
        }
    }
}