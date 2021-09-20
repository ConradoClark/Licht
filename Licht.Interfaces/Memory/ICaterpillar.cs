using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Memory
{
    public interface ICaterpillar<TObject>
    {
        int TailSize { get; set; }
        TObject Current { get; set; }
        TObject GetTrail(int index);
    }
}
