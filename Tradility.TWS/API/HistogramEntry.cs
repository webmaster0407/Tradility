/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tradility.TWS.API
{
    public struct HistogramEntry
    {
        public double Price { get; set; }
        public long Size { get; set; }
    }
}
