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
    [Table("Pricelist")]
    [DisplayName("Pricelist")]
    public class PricelistDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Pricelist")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("LookUpPriceListId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> LookUpPriceListId { get; set; }

        [Column("Percentage", Order = 5, TypeName = "FLOAT")]
        public Nullable<double> Percentage { get; set; }

        [Column("Status", Order = 6, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("ActivationDate", Order = 7, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ActivationDate { get; set; }

        [Column("DeactivationDate", Order = 8, TypeName = "DATETIME")]
        public Nullable<DateTime> DeactivationDate { get; set; }

        [Column("SalesTypeId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> SalesTypeId { get; set; }

        [Column("PricelistMasterId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_Pricelist_PricelistMaster")]
        [Association("PricelistMaster", "PricelistMasterId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]
        public Nullable<long> PricelistMasterId { get; set; }
        
        [Column("IsDeleted", Order = 11, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Type", Order = 12, TypeName = "SMALLINT")]
        public Nullable<short> Type { get; set; }

        [Column("DAId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
