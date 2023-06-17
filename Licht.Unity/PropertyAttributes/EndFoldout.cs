using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EndFoldoutAttribute : PropertyAttribute
    {
    }
}
