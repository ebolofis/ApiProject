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
    [Table("MealConsumption_Hist")]
    [DisplayName("MealConsumption_Hist")]
    public class MealConsumption_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_MealConsumption_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("GuestId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> GuestId { get; set; }

        [Column("ConsumedMeals", Order = 3, TypeName = "INT")]
        public Nullable<int> ConsumedMeals { get; set; }

        [Column("ConsumedTS", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ConsumedTS { get; set; }

        [Column("IsDeleted", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("BoardCode", Order = 6, TypeName = "NVARCHAR(150)")]
        public string BoardCode { get; set; }

        [Column("ReservationId", Order = 7, TypeName = "INT")]
        public Nullable<int> ReservationId { get; set; }

        [Column("ConsumedMealsChild", Order = 8, TypeName = "INT")]
        public Nullable<int> ConsumedMealsChild { get; set; }

        [Column("DepartmentId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> DepartmentId { get; set; }

        [Column("EndOfDayId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("PosInfoId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("Timezone", Order = 12, TypeName = "NVARCHAR(1)")]
        public string Timezone { get; set; }

        [Column("Room", Order = 13, TypeName = "NVARCHAR(50)")]
        public string Room { get; set; }

        [Column("HotelId", Order = 14, TypeName = "INT")]
        public int HotelId { get; set; }

        [Column("StaffId", Order = 15, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }
    }
}
