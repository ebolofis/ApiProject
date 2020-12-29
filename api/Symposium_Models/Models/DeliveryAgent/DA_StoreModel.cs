using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_StoreModel
    {
        public Nullable<long> Id { get; set; }

        public string Code { get; set; }
        [Required]
        /// <summary>
        /// Ονομασία καταστήματος
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ονομασία καταστήματος 2
        /// </summary>
        public string Title2 { get; set; }

        /// <summary>
        /// σχόλια καταστήματος
        /// </summary>
        public string Notes { get; set; }
        [Required]
        /// <summary>
        /// min
        /// </summary>
        public Nullable<int> DeliveryTime { get; set; }
        [Required]
        /// <summary>
        /// min
        /// </summary>
        public Nullable<int> TakeOutTime { get; set; }

        /// <summary>
        /// επίσημη ονομασία επιχείρησης
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// επίσημη ονομασία επιχείρησης 2
        /// </summary>
        public string Description2 { get; set; }

        /// <summary>
        /// ΔΟΥ
        /// </summary>
        public string Doy { get; set; }

        /// <summary>
        /// ΑΦΜ
        /// </summary>
        public string VatNo { get; set; }

        public string Email { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string Fax { get; set; }
        [Required]
        /// <summary>
        /// DA_Addresses.Id
        /// </summary>
        public Nullable<long> AddressId { get; set; }

        /// <summary>
        /// Εικόνα καταστήματος (path)
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Μικρή Εικόνα καταστήματος (path)
        /// </summary>
        public string Thumbnail { get; set; }
        [Required]
        /// <summary>
        /// WebApi base url
        /// </summary>
        public string WebApi { get; set; }
        [Required]
        /// <summary>
        /// Id Pos συνδεμένο με το delivery/web
        /// </summary>
        public Nullable<int> PosId { get; set; }
        [Required]
        /// <summary>
        /// Store Id (για επιλογή DB καταστήματος)
        /// </summary>
        public string StoreId { get; set; }
        [Required]
        /// <summary>
        /// Staff Id του καταστήματος (για παραγγελίες)
        /// </summary>
        public Nullable<int> PosStaffId { get; set; }
        [Required]
        /// <summary>
        /// store's  user username (Auth header)
        /// </summary>
        public string Username { get; set; }
        [Required]
        /// <summary>
        /// store's  user password (Auth header)
        /// </summary>
        public string Password { get; set; }
        [Required]
        /// <summary>
        /// 0:closed, 1:delivery only, 2: takeout only, 4: full operational
        /// </summary>
        public DAStoreStatusEnum StoreStatus { get; set; }

        /// <summary>
        /// Χρώμα καταστήματος για το UI
        /// </summary>
        public string RGB { get; set; }
        /// <summary>
        /// Timer Scheduler
        /// </summary>
        public  string CronScheduler { get; set; }

        public Boolean isAllowedToHaveCreditCard { get; set; } = true;


        public DA_StoreModel()
        {

            this.isAllowedToHaveCreditCard = true;
        }

    }

    public class DA_StoreInfoModel : DA_StoreModel
    {
        public Nullable<float> Latitude { get; set; }
        public Nullable<float> Longtitude { get; set; }
    }
}
