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
    [Table("InvoiceShippingDetails")]
    [DisplayName("InvoiceShippingDetails")]
    public class InvoiceShippingDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_InvoiceShippingDetails")]
        public long Id { get; set; }

        [Column("InvoicesId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_InvoiceShippingDetails_Invoices")]
        [Association("Invoices", "InvoicesId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> InvoicesId { get; set; }

        [Column("ShippingAddressId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> ShippingAddressId { get; set; }

        [Column("ShippingAddress", Order = 4, TypeName = "NVARCHAR(300)")]
        public string ShippingAddress { get; set; }

        [Column("ShippingCity", Order = 5, TypeName = "NVARCHAR(100)")]
        public string ShippingCity { get; set; }

        [Column("ShippingZipCode", Order = 6, TypeName = "NVARCHAR(50)")]
        public string ShippingZipCode { get; set; }

        [Column("BillingAddressId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> BillingAddressId { get; set; }

        [Column("BillingAddress", Order = 8, TypeName = "NVARCHAR(300)")]
        public string BillingAddress { get; set; }

        [Column("BillingCity", Order = 9, TypeName = "NVARCHAR(100)")]
        public string BillingCity { get; set; }
        [Column("BillingZipCode", Order = 10, TypeName = "NVARCHAR(50)")]
        public string BillingZipCode { get; set; }
        [Column("BillingName", Order = 11, TypeName = "NVARCHAR(100)")]
        public string BillingName { get; set; }
        [Column("BillingVatNo", Order = 12, TypeName = "NVARCHAR(100)")]
        public string BillingVatNo { get; set; }
        [Column("BillingDOY", Order = 13, TypeName = "NVARCHAR(100)")]
        public string BillingDOY { get; set; }
        [Column("BillingJob", Order = 14, TypeName = "NVARCHAR(100)")]
        public string BillingJob { get; set; }

        [Column("Floor", Order = 15, TypeName = "NVARCHAR(200)")]
        public string Floor { get; set; }

        [Column("CustomerRemarks", Order = 16, TypeName = "NVARCHAR(1500)")]
        public string CustomerRemarks { get; set; }

        [Column("StoreRemarks", Order = 17, TypeName = "NVARCHAR(1500)")]
        public string StoreRemarks { get; set; }

        [Column("CustomerID", Order = 18, TypeName = "BIGINT")]
        public Nullable<long> CustomerID { get; set; }

        [Column("CustomerName", Order = 19, TypeName = "NVARCHAR(500)")]
        public string CustomerName { get; set; }

        [Column("Longtitude", Order = 20, TypeName = "FLOAT")]
        public Nullable<double> Longtitude { get; set; }

        [Column("Latitude", Order = 21, TypeName = "FLOAT")]
        public Nullable<double> Latitude { get; set; }

        [Column("Phone", Order = 22, TypeName = "NVARCHAR(150)")]
        public string Phone { get; set; }
    }
}
