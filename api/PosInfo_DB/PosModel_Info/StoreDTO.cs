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
    [Table("Store")]
    [DisplayName("Store")]
    public class StoreDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Store")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("ExtDescription", Order = 3, TypeName = "NVARCHAR(MAX)")]
        public string ExtDescription { get; set; }

        [Column("Phone1", Order = 4, TypeName = "NVARCHAR(150)")]
        public string Phone1 { get; set; }

        [Column("Phone2", Order = 5, TypeName = "NVARCHAR(50)")]
        public string Phone2 { get; set; }

        [Column("Phone3", Order = 6, TypeName = "NVARCHAR(50)")]
        public string Phone3 { get; set; }

        [Column("Email", Order = 7, TypeName = "NVARCHAR(250)")]
        public string Email { get; set; }

        [Column("Address", Order = 8, TypeName = "NVARCHAR(150)")]
        public string Address { get; set; }

        [Column("Latitude", Order = 9, TypeName = "NVARCHAR(50)")]
        public string Latitude { get; set; }

        [Column("Longtitude", Order = 10, TypeName = "NVARCHAR(50)")]
        public string Longtitude { get; set; }

        [Column("ImageUri", Order = 11, TypeName = "NVARCHAR(250)")]
        public string ImageUri { get; set; }

        [Column("LogoUri", Order = 12, TypeName = "NVARCHAR(250)")]
        public string LogoUri { get; set; }

        [Column("Status", Order = 13, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("StoreMapId", Order = 14, TypeName = "UNIQUEIDENTIFIER")]
        public Nullable<System.Guid> StoreMapId { get; set; }

        [Column("StoreKitchenInstruction", Order = 15, TypeName = "SMALLINT")]
        public Nullable<short> StoreKitchenInstruction { get; set; }
    }
}
