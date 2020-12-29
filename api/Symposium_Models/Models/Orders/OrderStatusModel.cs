using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class OrderStatusModel
    {
        public long Id { get; set; }
        public Nullable<OrderStatusEnum> Status { get; set; }
        public Nullable<System.DateTime> TimeChanged { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<int> ExtState { get; set; }
        public Nullable<bool> IsSend { get; set; }
        public Nullable<long> DAOrderId { get; set; }

        public virtual OrderModel Order { get; set; }
        public virtual StaffModels Staff { get; set; }
    }

    public class OrderStatusFilter
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> Status { get; set; }
        public Nullable<System.DateTime> TimeChanged { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<int> ExtState { get; set; }
    }
}
