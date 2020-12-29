using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class RestaurantsModel
    {
        /// <summary>
        /// Id record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ελληνική Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string NameGR { get; set; }

        /// <summary>
        /// Αγγλική Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string NameEn { get; set; }

        /// <summary>
        /// Ρωσικη Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string NameRu { get; set; }

        /// <summary>
        /// Γαλλική Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string NameFr { get; set; }

        /// <summary>
        /// Γερμανική Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string NameDe { get; set; }

        /// <summary>
        /// Εικόνα του Ξενοδοχείου
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Ελληνική Περιγραφή του Ξενοδοχείου
        /// </summary>
        [Required]
        public string DescriptionGR { get; set; }

        /// <summary>
        /// Αγγλική Περιγραφή του Ξενοδοχείου
        /// </summary>
        [Required]
        public string DescriptionEn { get; set; }

        /// <summary>
        /// Ρωσική Περιγραφή του Ξενοδοχείου
        /// </summary>
        [Required]
        public string DescriptionRu { get; set; }

        /// <summary>
        /// Γαλλική Περιγραφή του Ξενοδοχείου
        /// </summary>
        [Required]
        public string DescriptionFr { get; set; }

        /// <summary>
        /// Γερμανική Περιγραφή του Ξενοδοχείου
        /// </summary>
        [Required]
        public string DescriptionDe { get; set; }
    }

    public class RestaurantsListModel
    {
        public List<RestaurantsModel> RestaurantsList { get; set; }
    }
}
