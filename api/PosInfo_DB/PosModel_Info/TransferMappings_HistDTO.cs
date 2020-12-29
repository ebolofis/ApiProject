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
    [Table("TransferMappings_Hist")]
    [DisplayName("TransferMappings_Hist")]
    public class TransferMappings_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_TransferMappings_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("PmsDepartmentId", Order = 2, TypeName = "NVARCHAR(100)")]
        public string PmsDepartmentId { get; set; }

        [Column("PmsDepDescription", Order = 3, TypeName = "NVARCHAR(250)")]
        public string PmsDepDescription { get; set; }

        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }

        [Column("SalesTypeId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> SalesTypeId { get; set; }

        [Column("VatPercentage", Order = 6, TypeName = "FLOAT")]
        public Nullable<double> VatPercentage { get; set; }

        [Column("PosDepartmentId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> PosDepartmentId { get; set; }

        [Column("PriceListId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> PriceListId { get; set; }

        [Column("VatCode", Order = 9, TypeName = "INT")]
        public Nullable<int> VatCode { get; set; }

        [Column("ProductCategoryId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("HotelId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> HotelId { get; set; }
    }
}
