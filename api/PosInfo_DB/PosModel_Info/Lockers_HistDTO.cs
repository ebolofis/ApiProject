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
    [Table("Lockers_Hist")]
    [DisplayName("Lockers_Hist")]
    public class Lockers_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_Lockers_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("HasLockers", Order = 2, TypeName = "BIT")]
        public Nullable<bool> HasLockers { get; set; }

        [Column("TotalLockers", Order = 3, TypeName = "FLOAT")]
        public Nullable<double> TotalLockers { get; set; }

        [Column("TotalLockersAmount", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> TotalLockersAmount { get; set; }

        [Column("Paidlockers", Order = 5, TypeName = "FLOAT")]
        public Nullable<double> Paidlockers { get; set; }

        [Column("PaidlockersAmount", Order = 6, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> PaidlockersAmount { get; set; }

        [Column("OccLockers", Order = 7, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> OccLockers { get; set; }

        [Column("OccLockersAmount", Order = 8, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> OccLockersAmount { get; set; }

        [Column("EndOfDayId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("PosInfoId", Order = 10, TypeName = "BIGINT")]
        public long PosInfoId { get; set; }

        [Column("TotalCash", Order = 11, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_TotalCash", NullDisplayText = "0")]
        public int TotalCash { get; set; }

        [Column("TotalSplashCash", Order = 12, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_TotalSplashCash", NullDisplayText = "0")]
        public int TotalSplashCash { get; set; }

        [Column("ReturnCash", Order = 13, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_ReturnCash", NullDisplayText = "0")]
        public int ReturnCash { get; set; }

        [Column("ReturnSplashCash", Order = 14, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_ReturnSplashCash", NullDisplayText = "0")]
        public int ReturnSplashCash { get; set; }

        [Column("CloseCash", Order = 15, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_CloseCash", NullDisplayText = "0")]
        public int CloseCash { get; set; }

        [Column("CloseSplashCash", Order = 16, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_Hist_CloseSplashCash", NullDisplayText = "0")]
        public int CloseSplashCash { get; set; }
    }
}
