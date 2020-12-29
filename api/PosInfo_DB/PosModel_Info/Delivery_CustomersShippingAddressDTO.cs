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
    [Table("Delivery_CustomersShippingAddress")]
    [DisplayName("Delivery_CustomersShippingAddress")]
    public class Delivery_CustomersShippingAddressDTO
    {
        [Column("ID", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Delivery_CustomersShippingAddress")]
        public long ID { get; set; }

        [Column("CustomerID", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Delivery_CustomersShippingAddress_Delivery_Customers")]
        [Association("Delivery_Customers", "CustomerID", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]//Cascade delete on customer
        public long CustomerID { get; set; }

        [Column("AddressStreet", Order = 3, TypeName = "NVARCHAR(70)")]
        [Required]
        public string AddressStreet { get; set; }

        [Column("AddressNo", Order = 4, TypeName = "NVARCHAR(200)")]
        public string AddressNo { get; set; }

        [Column("City", Order = 5, TypeName = "NVARCHAR(50)")]
        public string City { get; set; }

        [Column("Zipcode", Order = 6, TypeName = "NVARCHAR(10)")]
        public string Zipcode { get; set; }

        [Column("Type", Order = 7, TypeName = "INT")]
        [ForeignKey("FK_Delivery_CustomersShippingAddress_Delivery_AddressTypes")]
        [Association("Delivery_AddressTypes", "Type", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<int> Type { get; set; }

        [Column("Latitude", Order = 8, TypeName = "NVARCHAR(50)")]
        public string Latitude { get; set; }

        [Column("Longtitude", Order = 9, TypeName = "NVARCHAR(50)")]
        public string Longtitude { get; set; }

        [Column("Floor", Order = 10, TypeName = "NVARCHAR(200)")]
        public string Floor { get; set; }
        [Column("IsSelected", Order = 11, TypeName = "BIT")]
        public Nullable<bool> IsSelected { get; set; }

        [Column("IsDeleted", Order = 12, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("ExtKey", Order = 13, TypeName = "VARCHAR(250)")]
        public string ExtKey { get; set; }

        [Column("ExtType", Order = 14, TypeName = "INT")]
        public Nullable<int> ExtType { get; set; }

        [Column("ExtObj", Order = 15, TypeName = "VARCHAR(MAX)")]
        public string ExtObj { get; set; }

        [Column("ExtId1", Order = 16, TypeName = "NVARCHAR(50)")]
        public string ExtId1 { get; set; }

        [Column("ExtId2", Order = 17, TypeName = "NVARCHAR(50)")]
        public string ExtId2 { get; set; }

        [Column("VerticalStreet", Order = 18, TypeName = "NVARCHAR(80)")]
        public string VerticalStreet { get; set; }

        [Column("Notes", Order = 19, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }
    }
}
