using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public class DeliveryFilters
    {
        public long? OrderNo { get; set; }
        public long? OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public List<long?> SelectedSalesTypes { get; set; }
    }

    public class StatusCounts {
        public long? Status { get; set; }
        public long? OrdersCount { get; set; }
    }
}