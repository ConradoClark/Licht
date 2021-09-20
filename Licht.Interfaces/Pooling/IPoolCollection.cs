using System.Collections.Generic;

namespace Licht.Interfaces.Pooling
{
    public interface IPoolCollection
    {
        IEnumerable<IPool> GetPools { get; }
        void AddPool(IPool pool);
    }
}
