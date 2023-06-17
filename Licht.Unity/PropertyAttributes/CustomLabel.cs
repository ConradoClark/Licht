    using System;
using System.Collections.Generic;
using System.Text;
    using UnityEngine;

    namespace Licht.Unity.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class CustomLabelAttribute : PropertyAttribute
    {
        public string Text { get; set; }

        public CustomLabelAttribute(string text)
        {
            Text = text;
        }
    }
}
