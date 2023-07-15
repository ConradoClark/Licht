using System;
using UnityEngine.Events;

namespace Licht.Unity.Links
{
    public abstract class ObservableBindingReference : BindingReference
    {
        public UnityEvent<BindingReference> OnChange;
    }
}