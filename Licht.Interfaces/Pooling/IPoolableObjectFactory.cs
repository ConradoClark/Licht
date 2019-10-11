using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Pooling
{
    public interface IPoolableObjectFactory<out T> where T: IPoolableObject
    {
        T Instantiate();
    }
}
