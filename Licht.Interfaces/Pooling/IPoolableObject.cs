using Licht.Interfaces.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Pooling
{
    public interface IPoolableObject : IInitializable, IActivable, IDeactivable
    {
    }
}
