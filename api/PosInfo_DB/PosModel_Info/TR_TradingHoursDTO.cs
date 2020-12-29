using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("TR_TradingHours")]
    [DisplayName("TR_TradingHours")]
    public class TR_TradingHoursDTO
    {
        /// <summary>
        /// Time From ex: 23:30
        /// </summary>
        [Column("TimeFrom", Order = 1, TypeName = "TIME(7)")]
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        /// Time To ex: 23:30
        /// </summary>
        [Column("TimeTo", Order = 2, TypeName = "TIME(7)")]
        public TimeSpan TimeTo { get; set; }
    }
}
