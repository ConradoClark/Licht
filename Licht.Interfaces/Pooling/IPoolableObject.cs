using Licht.Interfaces.Update;

namespace Licht.Interfaces.Pooling
{
    public interface IPoolableObject : IInitializable, IActivable, IDeactivable
    {
        IPool Pool { get; set; }
    }
}
