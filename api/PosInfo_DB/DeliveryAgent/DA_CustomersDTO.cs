using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.DeliveryAgent
{
    [Table("DA_Customers")]
    [DisplayName("DA_Customers")]
    public class DA_CustomersDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [DisplayName("PK_DA_Customers")]
        public long ID { get; set; }

        [Column("Email", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Email { get; set; }

        [Column("Password", Order = 3, TypeName = "NVARCHAR(50)")]
        public string Password { get; set; }

        [Column("FirstName", Order = 4, TypeName = "NVARCHAR(100)")]
        public string FirstName { get; set; }

        [Column("LastName", Order = 5, TypeName = "NVARCHAR(100)")]
        [Required]
        public string LastName { get; set; }

        [Column("Phone1", Order = 6, TypeName = "NVARCHAR(20)")]
        public string Phone1 { get; set; }

        [Column("Phone2", Order = 7, TypeName = "NVARCHAR(20)")]
        public string Phone2 { get; set; }

        [Column("Mobile", Order = 8, TypeName = "NVARCHAR(20)")]
        public string Mobile { get; set; }

        [Column("BillingAddressesId", Order = 9, TypeName = "INT")]
        public int BillingAddressesId { get; set; }

        [Column("VatNo", Order = 10, TypeName = "NVARCHAR(20)")]
        public string VatNo { get; set; }

        [Column("Doy", Order = 11, TypeName = "NVARCHAR(100)")]
        public string Doy { get; set; }

        [Column("JobName", Order = 12, TypeName = "NVARCHAR(100)")]
        public string JobName { get; set; }

        [Column("LastAddressId", Order = 13, TypeName = "INT")]
        public int LastAddressId { get; set; }

        [Column("Notes", Order = 14, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }

        [Column("SecretNotes", Order = 15, TypeName = "NVARCHAR(1500)")]
        public string SecretNotes { get; set; }

        [Column("LastOrderNote", Order = 16, TypeName = "NVARCHAR(1500)")]
        public string LastOrderNotes { get; set; }
    }
}
