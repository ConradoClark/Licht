using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.UI
{
    [AddComponentMenu("L. Defaults: UI Camera")]
    public class UICamera : MonoBehaviour
    {
        [field:SerializeField]
        public Camera Camera { get; private set; }
    }
}
