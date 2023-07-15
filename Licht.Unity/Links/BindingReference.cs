using Licht.Unity.Objects;

namespace Licht.Unity.Links
{
    public abstract class BindingReference : BaseGameObject
    {
        public abstract object Get();
        public abstract void Set(object obj);
        public abstract void PreCacheBindings();
    }
}