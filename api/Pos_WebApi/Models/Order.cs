using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi
{
    public partial class Order
    {
        public long? PosInfoDetailId { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string RegNo { get; set; }
        public string RoomDescription { get; set; }
        public string RoomId { get; set; }
        public string DiscountRemark { get; set; }
        //invoicedelivery
        public long? ShippingAddressId { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingZipCode { get; set; }

        public long? BillingAddressId { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingZipCode { get; set; }
        public string Floor { get; set; }
        public string CustomerRemarks { get; set; }
        public string StoreRemarks { get; set; }
        public float Longtitude { get; set; }
        public float Latitude { get; set; }
        public string Phone { get; set; }

        ////New Delivery Fields
        //public string ExtKey { get; set; }
        //public int? ExtType { get; set; }
        //public string ExtObj { get; set; }
    }


    public class Receipt
    {
        public short? InvoiceIndex { get; set; }
        public string TableNo { get; set; }
        public long? RoomNo { get; set; }
        public string Waiter { get; set; }
        public long WaiterNo { get; set; }
        public long? PosId { get; set; }
        public string PosDescr { get; set; }
        public long? ReceiptNo { get; set; }
        public long Id { get; set; }
        public long? OrderNo { get; set; }
        public decimal? Total { get; set; }
        public decimal? Change { get; set; }
        public bool? IsVoid { get; set; }
        public List<ReceiptDetail> OrderDetail { get; set; }
        public decimal? TotalDiscount { get; set; }
        //public string PaidAmount { get; set; }
        public string ReceiptTypeDescription { get; set; }
        public string DepartmentTypeDescription { get; set; }
    }

    public class ReceiptDetail
    {
        public long Id { get; set; }
        public int? AA {get;set;}
        public decimal? Price { get; set; }
        public bool? IsChangeItem { get; set; }
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public double? Qty { get; set; }
        public string VatCode { get; set; }
        public string KitchenCode { get; set; }
        public string VatDesc { get; set; }
        public List<ReceiptExtra> OrderDetailIgredients { get; set; }

        public decimal? TotalAfterDiscount { get; set; }

        public decimal? Discount { get; set; }

        public Guid? Guid { get; set; }
    }

    public class ReceiptExtra
    {
        public decimal? Price { get; set; }
        public long? IngredientId { get; set; }
        public string Description { get; set; }
        public double? Qty { get; set; }
        public string VatCode { get; set; }
        public string VatDesc { get; set; }

        public decimal? TotalAfterDicount { get; set; }
    }


    public class AccountsObj
    {
        public long AccountId { get; set; }
        public Accounts Account { get; set; }
        public decimal Amount { get; set; }
        public long? GuestId { get; set; }
        public long? TransactionId { get; set; }
    }

    public partial class Transactions
    {
        public Guid? Guid { get; set; }
    }

}