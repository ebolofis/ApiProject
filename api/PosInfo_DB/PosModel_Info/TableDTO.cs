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
    [Table("Table")]
    [DisplayName("Table")]
    public class TableDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Table")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("SalesDescription", Order = 3, TypeName = "NVARCHAR(350)")]
        public string SalesDescription { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("MinCapacity", Order = 5, TypeName = "INT")]
        public Nullable<int> MinCapacity { get; set; }

        [Column("MaxCapacity", Order = 6, TypeName = "INT")]
        public Nullable<int> MaxCapacity { get; set; }

        [Column("RegionId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Table_Region")]
        [Association("Region", "RegionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> RegionId { get; set; }

        [Column("Status", Order = 8, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("YPos", Order = 9, TypeName = "NVARCHAR(100)")]
        public string YPos { get; set; }

        [Column("XPos", Order = 10, TypeName = "NVARCHAR(100)")]
        public string XPos { get; set; }

        [Column("IsOnline", Order = 11, TypeName = "BIT")]
        public Nullable<bool> IsOnline { get; set; }

        [Column("ReservationStatus", Order = 12, TypeName = "SMALLINT")]
        public Nullable<short> ReservationStatus { get; set; }

        [Column("Shape", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> Shape { get; set; }

        [Column("TurnoverTime", Order = 14, TypeName = "INT")]
        public Nullable<int> TurnoverTime { get; set; }

        [Column("ImageUri", Order = 15, TypeName = "NVARCHAR(500)")]
        public string ImageUri { get; set; }

        [Column("Width", Order = 16, TypeName = "FLOAT")]
        public Nullable<double> Width { get; set; }

        [Column("Height", Order = 17, TypeName = "FLOAT")]
        public Nullable<double> Height { get; set; }

        [Column("Angle", Order = 18, TypeName = "NVARCHAR(100)")]
        public string Angle { get; set; }

        [Column("IsDeleted", Order = 19, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
