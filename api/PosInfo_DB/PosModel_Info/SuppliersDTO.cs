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
    [Table("Suppliers")]
    [DisplayName("Suppliers")]
    public class SuppliersDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Suppliers")]
        public long Id { get; set; }

        [Column("Code", Order = 1, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("FullName", Order = 3, TypeName = "NVARCHAR(250)")]
        public string FullName { get; set; }

        [Column("VatNo", Order = 4, TypeName = "NVARCHAR(50)")]
        public string VatNo { get; set; }

        [Column("TaxOffice", Order = 5, TypeName = "NVARCHAR(250)")]
        public string TaxOffice { get; set; }

        [Column("Address", Order = 6, TypeName = "NVARCHAR(250)")]
        public string Address { get; set; }

        [Column("ZipCode", Order = 7, TypeName = "NVARCHAR(50)")]
        public string ZipCode { get; set; }

        [Column("Phone", Order = 8, TypeName = "NVARCHAR(50)")]
        public string Phone { get; set; }

        [Column("Email", Order = 9, TypeName = "NVARCHAR(250)")]
        public string Email { get; set; }
    }
}
