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
    [Table("Delivery_CustomersPhonesAndAddress")]
    [DisplayName("Delivery_CustomersPhonesAndAddress")]
    public class Delivery_CustomersPhonesAndAddressDTO
    {
        [Column("ID", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Delivery_CustomersPhonesAndAddress")]
        public long ID { get; set; }

        [Column("CustomerID", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Delivery_CustomersPhonesAndAddress_Delivery_Customers")]
        [Association("Delivery_Customers", "CustomerID", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        public long CustomerID { get; set; }

        [Column("PhoneID", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Delivery_CustomersPhonesAndAddress_Delivery_CustomersPhones")]
        [Association("Delivery_CustomersPhones", "PhoneID", "ID")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]//Cascade delete on customer
        public long PhoneID { get; set; }

        [Column("AddressID", Order = 4, TypeName = "BIGINT")]
        [Required]
        public long AddressID { get; set; }

        [Column("IsShipping", Order = 5, TypeName = "SMALLINT")]
        [Required]
        public short IsShipping { get; set; }
    }
}
