using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Objects
{
    public class DefaultMouseInputs : MonoBehaviour
    {
        [field:SerializeField]
        public InputActionReference MousePos { get; private set; }

        [field: SerializeField]
        public InputActionReference MouseLeftClick { get; private set; }

        [field: SerializeField]
        public InputActionReference MouseRightClick { get; private set; }

    }
}
