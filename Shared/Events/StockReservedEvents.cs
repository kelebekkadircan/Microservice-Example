﻿using Shared.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class StockReservedEvents  : IEvent
    {
        public Guid BuyerID { get; set; }

        public Guid OrderID { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
