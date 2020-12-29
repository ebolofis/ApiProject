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
    [Table("DA_LoyalPoints_Hist")]
    [DisplayName("DA_LoyalPoints_Hist")]
    public class DA_LoyalPointsHistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [Required]
        [DisplayName("PK_DA_LoyalPoints_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("CustomerId", Order = 3, TypeName = "BIGINT")]

        public long CustomerId { get; set; }

        /// <summary>
        /// πόντοι κερδισμένοι/εξαργύρωσης  (+/-)
        /// </summary>
        [Column("Points", Order = 4, TypeName = "INT")]
        public int Points { get; set; }

        [Column("Date", Order = 5, TypeName = "DATETIME")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Order Id (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία τότε OrderId=0)
        /// </summary>
        [Column("OrderId", Order = 6, TypeName = "BIGINT")]
        public long OrderId { get; set; }

        /// <summary>
        ///  Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0)
        /// </summary>
        [Column("StoreId", Order = 7, TypeName = "BIGINT")]
        public long StoreId { get; set; }


        /// <summary>
        ///  StaffId to track Loyal Points gained
        /// </summary>
        [Column("StaffId", Order = 8, TypeName = "BIGINT")]
        public long StaffId { get; set; }

        [Column("Description", Order = 9, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

    }
}
