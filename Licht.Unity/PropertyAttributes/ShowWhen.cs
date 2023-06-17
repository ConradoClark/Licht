using System;
using UnityEngine;

namespace Licht.Unity.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowWhenAttribute : PropertyAttribute
    {
        public bool Reverse { get; set; }
        public string FieldName { get; set; }

        public ShowWhenAttribute(string fieldName, bool reverse = false)
        {
            FieldName = fieldName;
            Reverse = reverse;
        }
    }

}
