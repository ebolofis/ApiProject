using Symposium.Models.Enums;
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
    [Table("DA_Stores")]
    [DisplayName("DA_Stores")]
    public class DA_StoresDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_Stores")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        /// <summary>
        /// Ονομασία καταστήματος
        /// </summary>
        [Column("Title", Order = 3, TypeName = "NVARCHAR(200)")]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Ονομασία καταστήματος 2
        /// </summary>
        [Column("Title2", Order = 4, TypeName = "NVARCHAR(200)")]
        public string Title2 { get; set; }

        /// <summary>
        /// σχόλια καταστήματος
        /// </summary>
        [Column("Notes", Order = 5, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }

        /// <summary>
        /// min
        /// </summary>
        [Column("DeliveryTime", Order = 6, TypeName = "INT")]
        [Required]
        public int DeliveryTime { get; set; }

        /// <summary>
        /// min
        /// </summary>
        [Column("TakeOutTime", Order = 7, TypeName = "INT")]
        [Required]
        public int TakeOutTime { get; set; }

        /// <summary>
        /// επίσημη ονομασία επιχείρησης
        /// </summary>
        [Column("Description", Order = 8, TypeName = "NVARCHAR(200)")]
        public string Description { get; set; }

        /// <summary>
        /// επίσημη ονομασία επιχείρησης 2
        /// </summary>
        [Column("Description2", Order = 9, TypeName = "NVARCHAR(200)")]
        public string Description2 { get; set; }

        /// <summary>
        /// ΔΟΥ
        /// </summary>
        [Column("Doy", Order = 10, TypeName = "NVARCHAR(100)")]
        public string Doy { get; set; }

        /// <summary>
        /// ΑΦΜ
        /// </summary>
        [Column("VatNo", Order = 11, TypeName = "NVARCHAR(20)")]
        public string VatNo { get; set; }

        [Column("Email", Order = 12, TypeName = "NVARCHAR(50)")]
        public string Email { get; set; }

        [Column("Phone1", Order = 13, TypeName = "NVARCHAR(20)")]
        public string Phone1 { get; set; }

        [Column("Phone2", Order = 14, TypeName = "NVARCHAR(20)")]
        public string Phone2 { get; set; }

        [Column("Fax", Order = 15, TypeName = "NVARCHAR(20)")]
        public string Fax { get; set; }

        /// <summary>
        /// DA_Addresses.Id
        /// </summary>
        [Column("AddressId", Order = 16, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_Stores_DA_Addresses")]
        [Association("DA_Addresses", "AddressId", "Id")]
        public Nullable<long> AddressId { get; set; }

        /// <summary>
        /// Εικόνα καταστήματος (path)
        /// </summary>
        [Column("Image", Order = 17, TypeName = "NVARCHAR(500)")]
        public string Image { get; set; }

        /// <summary>
        /// Μικρή Εικόνα καταστήματος (path)
        /// </summary>
        [Column("Thumbnail", Order = 18, TypeName = "NVARCHAR(500)")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// WebApi base url
        /// </summary>
        [Column("WebApi", Order = 19, TypeName = "NVARCHAR(500)")]
        [Required]
        public string WebApi { get; set; }

        /// <summary>
        /// Id Pos συνδεμένο με το delivery/web
        /// </summary>
        [Column("PosId", Order = 20, TypeName = "INT")]
        [Required]
        public int PosId { get; set; }

        /// <summary>
        /// Store Id (για επιλογή DB καταστήματος)
        /// </summary>
        [Column("StoreId", Order = 21, TypeName = "NVARCHAR(200)")]
        [Required]
        public string StoreId { get; set; }

        /// <summary>
        /// Staff Id του καταστήματος (για παραγγελίες)
        /// </summary>
        [Column("PosStaffId", Order = 22, TypeName = "INT")]
        [Required]
        public int PosStaffId { get; set; }

        /// <summary>
        /// store's  user username (Auth header)
        /// </summary>
        [Column("Username", Order = 23, TypeName = "NVARCHAR(50)")]
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// store's  user password (Auth header)
        /// </summary>
        [Column("Password", Order = 24, TypeName = "NVARCHAR(50)")]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 0:closed, 1:delivery only, 2: takeout only, 4: full operational
        /// </summary>
        [Column("StoreStatus", Order = 25, TypeName = "SMALLINT")]
        [Required]
        public DAStoreStatusEnum StoreStatus { get; set; }

        /// <summary>
        /// Χρώμα καταστήματος για το UI
        /// </summary>
        [Column("RGB", Order =26, TypeName ="NVARCHAR(10)")]
        public string RGB { get; set; }

        /// <summary>
        /// Scheduler ******  managed by BO -> Manage DA Stores
        /// </summary>
        [Column("CronScheduler", Order = 27, TypeName = "NVARCHAR(100)")]
        [DisplayFormatAttribute(DataFormatString = "DF_Da_Stores_CronScheduler", NullDisplayText = "''")]//DefaultValue (Name, Value)
        public string CronScheduler { get; set; }


        [Column("isAllowedToHaveCreditCard", Order = 28, TypeName = "BIT")]
        public Boolean isAllowedToHaveCreditCard { get; set; } = true;

    }
}
