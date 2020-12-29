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
    [Table("DA_LoyalPoints")]
    [DisplayName("DA_LoyalPoints")]
    public class DA_LoyalPointsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_LoyalPoints")]
        public long Id { get; set; }

        [Column("CustomerId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_LoyalPoints_DA_Customers")]
        [Association("DA_Customers", "CustomerId", "Id")]
        public long CustomerId { get; set; }

        /// <summary>
        /// πόντοι κερδισμένοι/εξαργύρωσης  (+/-)
        /// </summary>
        [Column("Points", Order = 3, TypeName = "INT")]
        [Required]
        public int Points { get; set; }

        [Column("Date", Order = 4, TypeName = "DATETIME")]
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Order Id (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία τότε OrderId=0)
        /// </summary>
        [Column("OrderId", Order = 5, TypeName = "BIGINT")]
        [Required]
        public long OrderId { get; set; }

        /// <summary>
        ///  Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0)
        /// </summary>
        [Column("StoreId", Order = 6, TypeName = "BIGINT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_LoyalPoints_StoreId", NullDisplayText = "0")]
        public long StoreId { get; set; }


        /// <summary>
        ///  StaffId to track Loyal Points gained
        /// </summary>
        [Column("StaffId", Order =7 , TypeName = "BIGINT")]
        [ForeignKey("FK_DA_LoyalPoints_Staff")]
        [Association("Staff", "StaffId", "Id")]
        public long StaffId { get; set; }

        [Column("Description", Order = 8, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

    }
}
