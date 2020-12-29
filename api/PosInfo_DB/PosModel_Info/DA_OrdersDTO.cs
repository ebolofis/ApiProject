using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("DA_Orders")]
    [DisplayName("DA_Orders")]
    public class DA_OrdersDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_Orders")]
        public long Id { get; set; }

        /// <summary>
        /// DA_Customers.Id
        /// </summary>
        [Column("CustomerId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_Orders_DA_Customers")]
        [Association("DA_Customers", "CustomerId", "Id")]
        public long CustomerId { get; set; }

        /// <summary>
        /// DA_Stores.Id
        /// </summary>
        [Column("StoreId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_Orders_DA_Stores")]
        [Association("DA_Stores", "StoreId", "Id")]
        public long StoreId { get; set; }

        /// <summary>
        /// DA_Stores.Code (Code καταστήματος). 
        /// One of StoreId and StoreCode should have value
        /// </summary>
        [Column("StoreCode", Order = 4, TypeName = "NVARCHAR(50)")]
        public string StoreCode { get; set; }

        /// <summary>
        /// DA_GeoPolygons.Id
        /// </summary>
        [Column("GeoPolygonId", Order = 4, TypeName = "BIGINT")]
        public long GeoPolygonId { get; set; }

        /// <summary>
        /// DA_Addresses.Id
        /// </summary>
        [Column("ShippingAddressId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_Orders_DA_Addresses")]
        [Association("DA_Addresses", "ShippingAddressId", "Id")]
        public long? ShippingAddressId { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα δημιουργίας παραγγελίας
        /// </summary>
        [Column("OrderDate", Order = 5, TypeName = "DATETIME")]
        [Required]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// εκτιμώμενη ημερομηνία/ώρα παράδωσης παραγγελίας
        /// </summary>
        [Column("EstBillingDate", Order = 6, TypeName = "DATETIME")]
        public Nullable<DateTime> EstBillingDate { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα παράδωσης παραγγελίας
        /// </summary>
        [Column("BillingDate", Order = 7, TypeName = "DATETIME")]
        public Nullable<DateTime> BillingDate { get; set; }

        /// <summary>
        /// εκτιμώμενη ημερομηνία/ώρα ΠΑΡΑΣΚΕΥΗΣ παραγγελίας
        /// </summary>
        [Column("EstTakeoutDate", Order = 8, TypeName = "DATETIME")]
        public Nullable<DateTime> EstTakeoutDate { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα ΠΑΡΑΣΚΕΥΗΣ παραγγελίας
        /// </summary>
        [Column("TakeoutDate", Order = 9, TypeName = "DATETIME")]
        public Nullable<DateTime> TakeoutDate { get; set; }

        /// <summary>
        /// ημερομηνια/ώρα που η παραγγελία έκλεισε (status: 4,5,6)
        /// </summary>
        [Column("FinishDate", Order = 10, TypeName = "DATETIME")]
        public Nullable<DateTime> FinishDate { get; set; }

        /// <summary>
        /// διάρκεια παραγγελίας (εφόσον έχει κλείσει)
        /// </summary>
        [Column("Duration", Order = 11, TypeName = "INT")]
        public Nullable<int> Duration { get; set; }

        /// <summary>
        /// Διεύθυνση επιχείρησης  (τιμολόγιο)
        /// </summary>
        [Column("BillingAddressId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> BillingAddressId { get; set; }

        /// <summary>
        /// Συνολικό ποσό (πριν την έκπτωση)
        /// </summary>
        [Column("Price", Order = 13, TypeName = "MONEY")]
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Ποσό έκπτωσης
        /// </summary>
        [Column("Discount", Order = 14, TypeName = "MONEY")]
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Συνολικό ποσό (μετά την έκπτωση)
        /// </summary>
        [Column("Total", Order = 15, TypeName = "MONEY")]
        [Required]
        public decimal Total { get; set; }

        /// <summary>
        /// 1=cash, credit/debit card=4, voucher=6
        /// </summary>
        [Column("AccountType", Order = 16, TypeName = "SMALLINT")]
        [Required]
        public Int16 AccountType { get; set; }

        /// <summary>
        /// 1=απόδειξη, 7=τιμολόγιο
        /// </summary>
        [Column("InvoiceType", Order = 17, TypeName = "SMALLINT")]
        [Required]
        public Int16 InvoiceType { get; set; }

        /// <summary>
        /// 20=delivary, 21=takeout
        /// </summary>
        [Column("OrderType", Order = 18, TypeName = "SMALLINT")]
        [Required]
        public OrderTypeStatus OrderType { get; set; }

        /// <summary>
        /// Κατάσταση παραγγελίας (Received = 0,Pending = 1,Preparing = 2,Ready = 3,Onroad = 4,Canceled = 5,Complete = 6)
        /// </summary>
        [Column("Status", Order = 19, TypeName = "SMALLINT")]
        [Required]
        public OrderStatusEnum Status { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα αλλαγής status
        /// </summary>
        [Column("StatusChange", Order = 20, TypeName = "DATETIME")]
        [Required]
        public DateTime StatusChange { get; set; }

        /// <summary>
        /// Σχόλια/παράπονα για τη παραγγελία (απο πελάτη κτλ)
        /// </summary>
        [Column("Remarks", Order = 21, TypeName = "NVARCHAR(1500)")]
        public string Remarks { get; set; }

        /// <summary>
        /// Order.Id που έδωσε το κατάστημα
        /// </summary>
        [Column("StoreOrderId", Order = 22, TypeName = "BIGINT")]
        [Required]
        public long StoreOrderId { get; set; }

        /// <summary>
        /// 0 αν το κατάστημα έχει ενημερωθεί για την παραγγελία
        /// </summary>
        [Column("IsSend", Order = 23, TypeName = "SMALLINT")]
        [Required]
        public Int16 IsSend { get; set; }

        /// <summary>
        /// προέλευση παραγγ.: 0: agent, 1: website, 2: mobile, 3: e-food
        /// </summary>
        [Column("Origin", Order = 24, TypeName = "SMALLINT")]
        [Required]
        public Int16 Origin { get; set; }

        /// <summary>
        /// json object
        /// </summary>
        [Column("ExtObj", Order = 25, TypeName = "TEXT")]
        public string ExtObj { get; set; }

        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Column("TotalVat", Order = 26, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Column("TotalTax", Order = 27, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total-TotalVat-TotalTax
        /// </summary>
        [Column("NetAmount", Order = 28, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// true: at least one item has changed
        /// </summary>
        [Column("ItemsChanged", Order = 29, TypeName = "BIT")]
        [Required]
        public bool ItemsChanged { get; set; }

        /// <summary>
        /// true: Order is paid
        /// </summary>
        [Column("IsPaid", Order = 30, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_Orders_IsPaid", NullDisplayText = "'False'")]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Order No from Client Store
        /// </summary>
        [Column("StoreOrderNo", Order = 31, TypeName = "BIGINT")]
        public Nullable<long> StoreOrderNo { get; set; }

        /// <summary>
        /// Point's gained by order
        /// </summary>
        [Column("PointsGain", Order = 32, TypeName = "INT")]
        public Nullable<int> PointsGain { get; set; }

        /// <summary>
        /// Point's redeemed by order
        /// </summary>
        [Column("PointsRedeem", Order = 33, TypeName = "INT")]
        public Nullable<int> PointsRedeem { get; set; }

        /// <summary>
        /// Point's redeemed by order
        /// </summary>
        [Column("IsDelay", Order = 34, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_Orders_IsDelay", NullDisplayText = "'False'")]
        public bool IsDelay { get; set; }

        /// <summary>
        /// Id from efood
        /// </summary>
        [Column("ExtId1", Order = 35, TypeName = "NVARCHAR(50)")]
        public string ExtId1 { get; set; }

        /// <summary>
        /// Id from external systems (ex: efood etc)
        /// </summary>
        [Column("ExtId2", Order = 36, TypeName = "NVARCHAR(50)")]
        public string ExtId2 { get; set; }

        /// <summary>
        /// Cover for order and invoice
        /// </summary>
        [Column("Cover", Order = 37, TypeName = "INT")]
        public Nullable<int> Cover { get; set; } 

        /// <summary>
        /// Error message if order can not added to store
        /// </summary>
        [Column("ErrorMessage", Order = 38, TypeName = "TEXT")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Order Data from external system, ex: e-food
        /// </summary>
        [Column("ExtData", Order = 39, TypeName = "TEXT")]
        public string ExtData { get; set; }

        /// <summary>
        /// Logical Error message etc. prices not same, different vats ....
        /// </summary>
        [Column("LogicErrors", Order = 40, TypeName = "TEXT")]
        public string LogicErrors { get; set; }

        /// <summary>
        /// Discount Remparks. Sended to Invoice Table on Store and on Field DiscountRemark
        /// </summary>
        [Column("DiscountRemark", Order = 41, TypeName = "NVARCHAR(500)")]
        public string DiscountRemark { get; set; }

        /// <summary>
        /// DA Agent Staff Id
        /// </summary>
        [Column("Staffid", Order = 42, TypeName = "BIGINT")]
        public Nullable<long> Staffid { get; set; }

        /// <summary>
        /// DA Agent Order Number
        /// </summary>
        [Column("AgentNo", Order = 43, TypeName = "NVARCHAR(50)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// External id of payment of bank transaction
        /// </summary>
        [Column("PaymentId", Order = 44, TypeName = "NVARCHAR(MAX)")]
        public string PaymentId { get; set; }

        /// <summary>
        /// Loyalty Code concerning DA Requests
        /// </summary>
        [Column("LoyaltyCode", Order = 45, TypeName = "NVARCHAR(200)")]
        public string LoyaltyCode { get; set; }
        /// <summary>
        ///TelephoneNumber concerning DA Requests
        /// </summary>
        [Column("TelephoneNumber", Order = 46, TypeName = "NVARCHAR(30)")]
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Metadata for web
        /// </summary>
        [Column("Metadata", Order = 47, TypeName = "NVARCHAR(MAX)")]
        public string Metadata { get; set; }

        /// <summary>
        /// Αριθμός παραγγελίας
        /// </summary>
        [Column("OrderNo", Order = 48, TypeName = "BIGINT")]
        public long OrderNo { get; set; }

        /// <summary>
        /// Id τραπεζιού
        /// </summary>
        [Column("TableId", Order = 49, TypeName = "BIGINT")]
        public Nullable<long> TableId { get; set; }

    }
}
