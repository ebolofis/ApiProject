using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Delivery
{
    public class DeliveryRoutingModel
    {
        public long Id { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public int Orders { get; set; }
        public Nullable<long> StaffId { get; set; }
        public string StaffName { get; set; }
        public Nullable<DateTime> AssignDate { get; set; }
        public Nullable<DateTime> AcceptDate { get; set; }
        public string RejectedNames { get; set; }
        public int Status { get; set; }
        public int AssignStatus { get; set; }
        public Nullable<DateTime> ReturnDate { get; set; }
        public Nullable<bool> Failure3th { get; set; }
        public Nullable<int> acceptTimeOffset { get; set; }
    }

    public class DeliveryRoutingHistModel : DeliveryRoutingModel
    {
        public int nYear { get; set; }
    }

    public class DeliveryRoutingIdModel
    {
        public long Id { get; set; }
    }

    public class DeliveryRoutingExtModel
    {
        public DeliveryRoutingModel route { get; set; }
        //public List<string> orderNos { get; set; }
        public List<string> orderNos { get; set; }
    }

    public class DeliveryRoutingLastRouteInfo
    {
        public DeliveryRoutingModel route { get; set; }
        //public List<string> orderNos { get; set; }
        public List<DeliveryRoutingLastRouteInfoDetails> ordersDetails { get; set; }
    }

    public class StaffRoutes
    {
        public List<DeliveryRoutingLastRouteInfo> staffRoutes { get; set; }
        public Nullable<long> StoreId { get; set; }
    }

    public class DeliveryRoutingLastRouteInfoDetails
    {
        public string orderNo { get; set; }
        public string orderId { get; set; }
        public Nullable<DateTime> deliveryTime { get; set; }
        public InvoiceShippingDetailsExtModel customerInfo { get; set; }
    }

    public class Routing3Model
    {
        public long Id { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<long> StoreId { get; set; }
        public int Status { get; set; }
        public int AssignStatus { get; set; }
        public Dictionary<string,string> Orders { get; set; }
        //public List<string> Orders { get; set; }
    }

    public class Routing3ReleaseModel
    {
        public long Id { get; set; }
        public long StaffId { get; set; }
        public long StoreId { get; set; }
    }
}
