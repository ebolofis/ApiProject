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
    [Table("Lockers")]
    [DisplayName("Lockers")]
    public class LockersDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_dbo.Lockers")]
        public long Id { get; set; }

        [Column("HasLockers", Order = 2, TypeName = "BIT")]
        [Required]
        public bool HasLockers { get; set; }

        [Column("TotalLockers", Order = 3, TypeName = "FLOAT")]
        [Required]
        public double TotalLockers { get; set; }

        [Column("TotalLockersAmount", Order = 4, TypeName = "FLOAT")]
        [Required]
        public double TotalLockersAmount { get; set; }

        [Column("Paidlockers", Order = 5, TypeName = "FLOAT")]
        [Required]
        public double Paidlockers { get; set; }

        [Column("PaidlockersAmount", Order = 6, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal PaidlockersAmount { get; set; }

        [Column("OccLockers", Order = 7, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal OccLockers { get; set; }

        [Column("OccLockersAmount", Order = 8, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal OccLockersAmount { get; set; }

        [Column("EndOfDayId", Order = 9, TypeName = "BIGINT")]
        [ForeignKey("FK_Lockers_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("PosInfoId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_Lockers_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public long PosInfoId { get; set; }

        [Column("TotalCash", Order = 11, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_TotalCash", NullDisplayText = "0")]
        public int TotalCash { get; set; }

        [Column("TotalSplashCash", Order = 12, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_TotalSplashCash", NullDisplayText = "0")]
        public int TotalSplashCash { get; set; }

        [Column("ReturnCash", Order = 13, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_ReturnCash", NullDisplayText = "0")]
        public int ReturnCash { get; set; }

        [Column("ReturnSplashCash", Order = 14, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_ReturnSplashCash", NullDisplayText = "0")]
        public int ReturnSplashCash { get; set; }

        [Column("CloseCash", Order = 15, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_CloseCash", NullDisplayText = "0")]
        public int CloseCash { get; set; }

        [Column("CloseSplashCash", Order = 16, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Lockers_CloseSplashCash", NullDisplayText = "0")]
        public int CloseSplashCash { get; set; }
    }
}
