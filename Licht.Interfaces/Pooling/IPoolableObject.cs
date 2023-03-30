using Licht.Interfaces.Update;

namespace Licht.Interfaces.Pooling
{
    public interface IPoolableObject : ICanInitialize, ICanActivate, ICanDeactivate
    {
        IPool Pool { get; set; }
    }
}
