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
    [Table("Delivery_CustomersPhones")]
    [DisplayName("Delivery_CustomersPhones")]
    public class Delivery_CustomersPhonesDTO
    {
        [Column("ID", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Delivery_CustomersPhones")]
        public long ID { get; set; }

        [Column("CustomerID", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Delivery_CustomersPhones_Delivery_Customers")]
        [Association("Delivery_Customers", "CustomerID", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]//Cascade delete on customer
        public long CustomerID { get; set; }

        [Column("PhoneNumber", Order = 3, TypeName = "NVARCHAR(20)")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column("PhoneType", Order = 4, TypeName = "INT")]
        [Required]
        [ForeignKey("FK_Delivery_CustomersPhones_Delivery_PhoneTypes")]
        [Association("Delivery_PhoneTypes", "PhoneType", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        public int PhoneType { get; set; }

        [Column("IsSelected", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsSelected { get; set; }

        [Column("ExtKey", Order = 6, TypeName = "VARCHAR(250)")]
        public string ExtKey { get; set; }

        [Column("ExtType", Order = 7, TypeName = "INT")]
        public Nullable<int> ExtType { get; set; }

        [Column("ExtObj", Order = 8, TypeName = "VARCHAR(MAX)")]
        public string ExtObj { get; set; }
    }
}
