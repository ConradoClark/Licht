using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Links
{
    [AddComponentMenu("L. Link: Binding by Path")]
    public class BindingReferenceByPath : BindingReference
    {
        [field: SerializeField] public Component Component { get; private set; }

        [field: SerializeField] public string BindingPath { get; private set; }
    
        private static Dictionary<BindingReferenceByPath, Func<object>> GetterCache { get; }
        private static Dictionary<BindingReferenceByPath, Action<object>> SetterCache { get; }

        static BindingReferenceByPath()
        {
            GetterCache = new Dictionary<BindingReferenceByPath, Func<object>>();
            SetterCache = new Dictionary<BindingReferenceByPath, Action<object>>();
        }

        protected virtual Func<object> GetGetter(string path)
        {
            if (path == BindingPath && GetterCache.ContainsKey(this))
            {
                return GetterCache[this];
            }
        
            path ??= BindingPath;
            var componentType = Component.GetType();
            object currentObject = Component;
            Func<object> compiledDelegate = null;

            foreach (var part in path.Split(".", StringSplitOptions.RemoveEmptyEntries))
            {
                var propertyInfo = componentType.GetProperty(part);
                if (propertyInfo != null)
                {
                    var getMethod = propertyInfo.GetGetMethod();
                
                    var objectInstance = Expression.Constant(currentObject); // Provide the value of the parameter here

                    var parameters = getMethod.GetParameters();

                    var methodCall = Expression.Call(
                        Expression.Convert(objectInstance, getMethod.DeclaringType!),
                        getMethod,
                        parameters.Length > 0
                            ? new Expression[] { Expression.Convert(objectInstance, parameters[0].ParameterType) }
                            : Array.Empty<Expression>()
                    );
                    var lambda = Expression.Lambda<Func<object>>(
                        Expression.Convert(methodCall, typeof(object))
                    );

                    compiledDelegate = lambda.Compile();
                    currentObject = compiledDelegate();
                    componentType = currentObject.GetType();
                    continue;
                }

                var fieldInfo = componentType.GetField(part);
                if (fieldInfo == null) throw new Exception($"Found no field or property path '{BindingPath}'");

                var instanceExpr = Expression.Constant(currentObject);
                var fieldExpr = Expression.Field(instanceExpr, fieldInfo);
                // Convert the field expression to object type
                var convertExpr = Expression.Convert(fieldExpr, typeof(object));

                // Create a lambda expression with the converted expression as the body
                var lambdaExpr = Expression.Lambda<Func<object>>(convertExpr);

                // Compile the lambda expression to create a delegate representing the getter
                compiledDelegate = lambdaExpr.Compile();
                currentObject = compiledDelegate();
                componentType = currentObject.GetType();
            }

            if (path == BindingPath) GetterCache[this] = compiledDelegate;
            return compiledDelegate;
        }

        protected virtual Action<object> GetSetter()
        {
            if (SetterCache.ContainsKey(this))
            {
                return SetterCache[this];
            }
        
            var componentType = Component.GetType();
            object currentObject = Component;
            Action<object> compiledDelegate = null;
            Expression convertValueExpr;
        
            var split = BindingPath.Split(".", StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                var subset = split.Take(split.Length - 1).ToArray();
                var getter = GetGetter(string.Join(".", subset));
                currentObject = getter();
                componentType = currentObject.GetType();
            }

            var lastPart = split.Last();
            var propertyInfo = componentType.GetProperty(lastPart);
            if (propertyInfo != null)
            {
                var setMethod = propertyInfo.GetSetMethod();
                
                var objectInstance = Expression.Constant(currentObject); // Provide the value of the parameter here
                var parameters = setMethod.GetParameters();
                var valueParameter = Expression.Parameter(typeof(object), "value");

                if (parameters.Length>0 && parameters[0].ParameterType== typeof(string))
                {
                    // Create a method call expression for calling ToString on the object
                    var toStringMethod = typeof(object).GetMethod("ToString");
                    convertValueExpr = Expression.Call(valueParameter, toStringMethod!);
                }
                else
                {
                    // Convert the value parameter to the field's type
                    convertValueExpr = Expression.Convert(valueParameter, parameters[0].ParameterType);
                }
            
                var methodCall = Expression.Call(
                    objectInstance,
                    setMethod,
                    parameters.Length > 0
                        ? new[] { convertValueExpr }
                        : Array.Empty<Expression>()
                );
                var lambda = Expression.Lambda<Action<object>>(
                    methodCall, valueParameter
                );

                compiledDelegate = lambda.Compile();
                SetterCache[this] = compiledDelegate;
                return compiledDelegate;
            }

            var fieldInfo = componentType.GetField(lastPart);
            if (fieldInfo == null) throw new Exception($"Found no field or property path '{BindingPath}'");

            var instanceExpr = Expression.Constant(currentObject);
            // Create a parameter expression for the value to be set
            var valueParam = Expression.Parameter(typeof(object), "value");

            if (fieldInfo.FieldType == typeof(string))
            {
                // Create a method call expression for calling ToString on the object
                var toStringMethod = typeof(object).GetMethod("ToString");
                convertValueExpr = Expression.Call(valueParam, toStringMethod!);
            }
            else
            {
                // Convert the value parameter to the field's type
                convertValueExpr = Expression.Convert(valueParam, fieldInfo.FieldType);
            }

            // Create an assignment expression to set the field value
            var assignmentExpr = Expression.Assign(Expression.Field(instanceExpr, fieldInfo), convertValueExpr);

            // Create a lambda expression with the assignment expression as the body
            LambdaExpression lambdaExpr = Expression.Lambda<Action<object>>(assignmentExpr, valueParam);

            // Compile the lambda expression to create a delegate representing the setter
            compiledDelegate = (Action<object>)lambdaExpr.Compile();
        
            SetterCache[this] = compiledDelegate;
            return compiledDelegate;
        }

        public override void PreCacheBindings()
        {
            GetGetter(null);
            GetSetter();
        }
    
        public override object Get()
        {
            return GetGetter(null)();
        }

        public override void Set(object obj)
        {
            GetSetter()(obj);
        }
    }
}