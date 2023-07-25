using System;
using UnityEngine;

namespace Licht.Unity.Generation
{
    [Serializable]
    public class ProcGenObjectStack<T>
    {
        [field:SerializeField]
        public T[] Objects { get; set; }
    }
}