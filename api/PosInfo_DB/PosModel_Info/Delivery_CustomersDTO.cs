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
    [Table("Delivery_Customers")]
    [DisplayName("Delivery_Customers")]
    public class Delivery_CustomersDTO
    {
        [Column("ID", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Delivery_Customers")]
        public long ID { get; set; }

        [Column("LastName", Order = 2, TypeName = "NVARCHAR(80)")]
        [Required]
        public string LastName { get; set; }

        [Column("FirstName", Order = 3, TypeName = "NVARCHAR(50)")]
        public string FirstName { get; set; }

        [Column("VatNo", Order = 4, TypeName = "NVARCHAR(20)")]
        public string VatNo { get; set; }

        [Column("DOY", Order = 5, TypeName = "NVARCHAR(50)")]
        public string DOY { get; set; }

        [Column("Floor", Order = 6, TypeName = "NVARCHAR(200)")]
        public string Floor { get; set; }

        [Column("email", Order = 7, TypeName = "NVARCHAR(255)")]
        public string email { get; set; }

        [Column("Comments", Order = 8, TypeName = "NVARCHAR(1500)")]
        public string Comments { get; set; }

        [Column("CustomerType", Order = 9, TypeName = "INT")]
        [ForeignKey("FK_Delivery_Customers_Delivery_CustomersTypes")]
        [Association("Delivery_CustomersTypes", "CustomerType", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<int> CustomerType { get; set; }

        [Column("BillingName", Order = 10, TypeName = "NVARCHAR(80)")]
        public string BillingName { get; set; }

        [Column("BillingVatNo", Order = 11, TypeName = "NVARCHAR(20)")]
        public string BillingVatNo { get; set; }

        [Column("BillingDOY", Order = 12, TypeName = "NVARCHAR(50)")]
        public string BillingDOY { get; set; }

        [Column("BillingJob", Order = 13, TypeName = "NVARCHAR(80)")]
        public string BillingJob { get; set; }

        [Column("DefaultPricelistId", Order = 14, TypeName = "BIGINT")]
        public Nullable<long> DefaultPricelistId { get; set; }

        [Column("IsDeleted", Order = 15, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("ExtCustId", Order = 16, TypeName = "NVARCHAR(255)")]
        public string ExtCustId { get; set; }

        [Column("ExtType", Order = 17, TypeName = "INT")]
        public Nullable<int> ExtType { get; set; }

        [Column("ExtObj", Order = 18, TypeName = "VARCHAR(MAX)")]
        public string ExtObj { get; set; }

        [Column("GDPR_IsDeleted", Order = 19, TypeName = "BIT")]
        public Nullable<bool> GDPR_IsDeleted { get; set; }

        [Column("GDPR_Marketing", Order = 20, TypeName = "BIT")]
        public Nullable<bool> GDPR_Marketing { get; set; }

        [Column("GDPR_Returner", Order = 21, TypeName = "BIT")]
        public Nullable<bool> GDPR_Returner { get; set; }

        [Column("GDPR_StaffId", Order = 22, TypeName = "BIT")]
        public Nullable<bool> GDPR_StaffId { get; set; }

        [Column("GDPR_StaffName", Order = 23, TypeName = "BIT")]
        public Nullable<bool> GDPR_StaffName { get; set; }

        [Column("PhoneComp", Order = 24, TypeName = "NVARCHAR(20)")]
        public string PhoneComp { get; set; }

        [Column("SendSms", Order = 25, TypeName = "BIT")]
        public Nullable<bool> SendSms { get; set; }

        [Column("SendEmail", Order = 26, TypeName = "BIT")]
        public Nullable<bool> SendEmail { get; set; }

        [Column("Proffesion", Order = 27, TypeName = "NVARCHAR(100)")]
        public string Proffesion { get; set; }

    }
}
