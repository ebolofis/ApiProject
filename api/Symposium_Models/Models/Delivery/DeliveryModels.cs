using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class DeliveryFilters
    {
        public Nullable<long> OrderNo { get; set; }
        public Nullable<long> OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public List<long> SelectedSalesTypes { get; set; }
        public List<long> ExternalTypes { get; set; }
        public Nullable<long> ExtType { get; set; }
    }

    public class StatusCounts
    {
        public long? Status { get; set; }
        public long? OrdersCount { get; set; }
    }

    public class InvoiceCoordinates
    {
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public Nullable<long> InvoiceId { get; set; }
        public Nullable<decimal> Longtitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
    }

    /// <summary>
    /// Model to singal back when a customer has changed
    /// </summary>
    public class DeliveryCustomerSignalModel
    {
        /// <summary>
        /// Updated or inserted ID after apply on db
        /// </summary>
        public Nullable<long> CustomerID { get; set; }
        /// <summary>
        /// GuestId from guest Mapped with DeliveryCustomer 
        /// </summary>
        public Nullable<long> GuestID { get; set; }

        /// <summary>
        /// Id from ShippingAddress Mapped with DeliveryCustomer 
        /// </summary>
        public Nullable<long> ShippingAddressId { get; set; }

        /// <summary>
        /// Id from BillingAddress Mapped with DeliveryCustomer 
        /// </summary>
        public Nullable<long> BillingAddressId { get; set; }

        /// <summary>
        /// Id from PhoneId Mapped with DeliveryCustomer 
        /// </summary>
        public Nullable<long> PhoneId { get; set; }

        /// <summary>
        /// Enum DeliveryCustomerSignalCause difines action triggered signal Cause
        /// </summary>
        public Nullable<int> Cause { get; set; }

    }

    public class DeliveryStatusOrders
    {
        public string AA { get; set; }
        //Order
        public long OrderId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        //Invoice
        public Nullable<long> InvoiceId { get; set; }
        public string InvoiceAbbr { get; set; }
        public Nullable<int> InvoiceCounter { get; set; }
        public Nullable<short> InvoiceType { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<DateTime> ReceivedDate { get; set; }
        public Nullable<DateTime> DeliveryTime { get; set; }

        public Nullable<byte> IsPaid { get; set; }

        public Nullable<bool> DA_IsPaid { get; set; }
        public Nullable<int> DA_Origin { get; set; }
        public Nullable<byte> PaidStatus { get; set; }

        public Nullable<bool> IsVoided { get; set; }
        public Nullable<decimal> Total { get; set; }

        //Customer
        public string CustomerName { get; set; }
        public Nullable<long> CustomerID { get; set; }

        //Shipping
        public Nullable<long> AddressId { get; set; }
        public string Address { get; set; }
        public string Floor { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public bool hasDelay { get; set; }


        public Nullable<decimal> Longtitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }

        //OrderStatus
        public Nullable<long> OrderStatusId { get; set; }
        public long CurrentStatus { get; set; }
        public long DeliveryState { get; set; }
        public Nullable<DateTime> StatusChangedTS { get; set; }
        public string Pricelists { get; set; }
        public bool? IsDelay { get; set; }

        //Staff
        public Nullable<long> StaffId { get; set; }
        public string StaffName { get; set; }
        //Staff Status
        public Nullable<long> StatusStaffId { get; set; }
        public string StatusStaffName { get; set; }

        public int? ExtType { get; set; }
        public string ExtObj { get; set; }

        public long? ExtKey { get; set; }

        public Nullable<long> OrderNum { get; set; }
        public Nullable<long> nType { get; set; }

        public Nullable<int> StatusTimeDifference { get; set; }

        public Nullable<int> OrderTotalTime { get; set; }

        public Nullable<int> NewStoreTime { get; set; }

        public Nullable<int> NewDistributionTime { get; set; }

        /// <summary>
        /// DA Notes For Order
        /// </summary>
        public string OrderNotes { get; set; }

        /// <summary>
        /// DA Notes For Store
        /// </summary>
        public string StoreNotes { get; set; }

        /// <summary>
        /// DA Notes For Customer
        /// </summary>
        public string CustomerNotes { get; set; }

        /// <summary>
        /// DA Secret Notes For Customer
        /// </summary>
        public string CustomerSecretNotes { get; set; }

        /// <summary>
        /// DA Last Order Notes For Customer
        /// </summary>
        public string CustomerLastOrderNotes { get; set; }

        public string LogicErrors { get; set; }

        public long DeliveryRoutingId { get; set; }

        public List<OrderStatusModel> orderStatusModel;
    }

    public class DeliveryOrdersSignal
    {
        //Order
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public Nullable<long> InvoiceId { get; set; }
        public Nullable<int> InvoiceCounter { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Address { get; set; }
        public string Floor { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

    }
}
