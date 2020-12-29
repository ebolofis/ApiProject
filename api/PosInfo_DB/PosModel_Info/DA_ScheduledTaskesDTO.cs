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
    [Table("DA_ScheduledTaskes")]
    [DisplayName("DA_ScheduledTaskes")]
    public class DA_ScheduledTaskesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_ScheduledTaskes")]
        public long Id { get; set; }

        /// <summary>
        /// Το full url για τον κάθε πίνακα, Ο controller που θα κληθεί
        /// </summary>
        [Column("StoreFullURL", Order = 2, TypeName = "NVARCHAR(2000)")]
        [Required]
        public string TableDescr { get; set; }

        [Column("TableId", Order = 3, TypeName = "BIGINT")]
        [Required]
        public long TableId { get; set; }

        [Column("StoreId", Order = 4, TypeName = "BIGINT")]
        [Required]
        public long StoreId { get; set; }

        /// <summary>
        /// 0:ins,1:del,2:upd
        /// </summary>
        [Column("Action", Order = 4, TypeName = "INT")]
        [Required]
        public int Action { get; set; }

        /// <summary>
        /// Σειρά με την οποία θα κατέβουν οι πίνακες στο κατάστημα
        /// </summary>
        [Column("Short", Order = 5, TypeName = "INT")]
        [Required]
        public int Short { get; set; }

        /// <summary>
        /// Αριθμός αποτυχιών
        /// </summary>
        [Column("FaildNo", Order = 6, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_ScheduledTaskes_FaildNo", NullDisplayText = "0")]
        public int FaildNo { get; set; }
    }
}
