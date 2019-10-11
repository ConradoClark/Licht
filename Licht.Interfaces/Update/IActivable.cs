using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Update
{
    public interface IActivable : IActivationReportable
    {
        bool Activate();
    }
}
