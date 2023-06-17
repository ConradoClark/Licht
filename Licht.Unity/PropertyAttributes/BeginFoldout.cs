using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BeginFoldoutAttribute : PropertyAttribute
    {
        public string Name { get; set; }

        public BeginFoldoutAttribute(string name) => Name = name;
    }
}
