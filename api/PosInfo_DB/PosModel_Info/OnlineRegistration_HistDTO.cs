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
    [Table("OnlineRegistration_Hist")]
    [DisplayName("OnlineRegistration_Hist")]
    public class OnlineRegistration_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_OnlineRegistration_Hist")]
        [Association("nYear", "BarCode", "")]
        public int nYear { get; set; }

        [Column("BarCode", Order = 1, TypeName = "NVARCHAR(100)")]
        [Required]
        public string BarCode { get; set; }

        [Column("FirtName", Order = 2, TypeName = "NVARCHAR(100)")]
        [Required]
        public string FirtName { get; set; }

        [Column("LastName", Order = 3, TypeName = "NVARCHAR(100)")]
        [Required]
        public string LastName { get; set; }

        [Column("Mobile", Order = 4, TypeName = "NVARCHAR(100)")]
        [Required]
        public string Mobile { get; set; }

        [Column("Dates", Order = 5, TypeName = "INT")]
        [Required]
        public int Dates { get; set; }

        [Column("Children", Order = 6, TypeName = "INT")]
        [Required]
        public int Children { get; set; }

        [Column("Adults", Order = 7, TypeName = "INT")]
        [Required]
        public int Adults { get; set; }

        [Column("PaymentType", Order = 8, TypeName = "INT")]
        [Required]
        public int PaymentType { get; set; }

        [Column("ChildTicket", Order = 9, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal ChildTicket { get; set; }

        [Column("AdultTicket", Order = 10, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal AdultTicket { get; set; }

        [Column("RegistrationDate", Order = 11, TypeName = "DATETIME")]
        [Required]
        public System.DateTime RegistrationDate { get; set; }

        [Column("TotalChildren", Order = 12, TypeName = "INT")]
        [Required]
        public int TotalChildren { get; set; }

        [Column("TotalAdults", Order = 13, TypeName = "INT")]
        [Required]
        public int TotalAdults { get; set; }

        [Column("ChildrenEntered", Order = 14, TypeName = "INT")]
        [Required]
        public int ChildrenEntered { get; set; }

        [Column("AdultsEntered", Order = 15, TypeName = "INT")]
        [Required]
        public int AdultsEntered { get; set; }

        [Column("TotalAmount", Order = 16, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal TotalAmount { get; set; }

        [Column("RemainingAmount", Order = 17, TypeName = "DECIMAL(18,2)")]
        [Required]
        public decimal RemainingAmount { get; set; }

        [Column("Status", Order = 18, TypeName = "INT")]
        [Required]
        public int Status { get; set; }

        [Column("Email", Order = 19, TypeName = "NVARCHAR(100)")]
        public string Email { get; set; }

        [Column("OrderNumber", Order = 20, TypeName = "NVARCHAR(80)")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// if false then anonymize user when status is finished
        /// </summary>
        [Column("Gdpr", Order = 21, TypeName = "BIT")]
        public bool Gdpr { get; set; }


        [Column("isAnonymized", Order = 22, TypeName = "BIT")]
        public bool isAnonymized { get; set; }
    }
}
