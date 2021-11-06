﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Generation
{
    public interface ISeedable<TSeed>
    {
        TSeed Seed { get; set; }
    }
}