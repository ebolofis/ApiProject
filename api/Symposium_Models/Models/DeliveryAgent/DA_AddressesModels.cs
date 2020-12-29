using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_AddressModel
    {
        [Required]
        [Range(0, long.MaxValue)]
        public long Id { get; set; }
        /// <summary>
        /// CustomerId or StoreId
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public long OwnerId { get; set; }
        [Required]
        [MaxLength(200)]
        public string AddressStreet { get; set; }
        [MaxLength(20)]
        public string AddressNo { get; set; }

        /// <summary>
        /// κάθετος δρόμος
        /// </summary>
        [MaxLength(200)]
        public string VerticalStreet { get; set; }
        [MaxLength(200)]
        public string Floor { get; set; }
        /// <summary>
        /// ex: Kallithea
        /// </summary>
        [MaxLength(100)]
        [MinLength(2)]
        public string Area { get; set; }
        /// <summary>
        /// ex: Athina
        /// </summary>
        [MaxLength(100)]
        [MinLength(2)]
        public string City { get; set; }
        [MaxLength(10)]
        [MinLength(5)]
        public string Zipcode { get; set; }
        [Required]
        public Nullable<float> Latitude { get; set; }
        [Required]
        public Nullable<float> Longtitude { get; set; }

        /// <summary>
        /// σχόλια (εμφάνιση στο ΔΠ)
        /// </summary>
        [MaxLength(1500)]
        public string Notes { get; set; }

        /// <summary>
        /// περιγραφή κουδουνιού (εμφάνιση στο ΔΠ)
        /// </summary>
        [MaxLength(200)]
        public string Bell { get; set; }

        /// <summary>
        /// 0:Shipping, 1: Billing, 2: Store
        /// </summary>
        [Required]
        [Range(0, 2)]
        public int AddressType { get; set; }

        public string ExtObj { get; set; }

        /// <summary>
        /// e-food id
        /// </summary>
        public string ExtId1 { get; set; }

        public string ExtId2 { get; set; }
        [MaxLength(200)]
        public string FriendlyName { get; set; }
        public bool IsDeleted { get; set; }
        public string LastPhoneNumber { get; set; }

        /// <summary>
        /// Sent from Service. true: Is Shipping, false: Is Billing
        /// </summary>
        public Nullable<bool> isShipping { get; set; }

        /// <summary>
        /// Helper method
        /// </summary>
        public string NotesHlp { get; set; }

        /// <summary>
        /// Describes Address Proximity 0 (the worst Result) to 3 (the best result).
        /// 0:APPROXIMATE, 1: GEOMETRIC_CENTER, 2: RANGE_INTERPOLATED, 3: ROOFTOP
        /// </summary>
        public AddressProximityEnum AddressProximity { get; set; } = AddressProximityEnum.Unown_Proximity;


        /// <summary>
        /// Search Address From Map Plugin
        /// </summary>
        public string SearchAddressString { get; set; }

       
      //  public string Phonetics { get; set; }

        
     //   public string PhoneticsArea { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string addressInfo = Id.ToString() + ": " + (AddressStreet ?? "") + " " + (AddressNo ?? "") + ", " + (City ?? "") + ", " + (Area ?? "") + ", " + (Zipcode ?? "");
            return addressInfo;
        }

    }

    public class DA_AddressPhoneModel
    {
        public long CustomerId { get; set; }
        public long AddressId { get; set; }
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// model for Photetics (Address and Area)
    /// </summary>
    public class DA_AddressPhoneticModel
    {
        public long Id { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public string VerticalStreet { get; set; }
        public string Area { get; set; }
        public string Phonetics { get; set; }
        public string PhoneticsArea { get; set; }
    }
}
