using Microsoft.SqlServer.Types;
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
    [DisplayName("DA_Addresses")]
    [Table("DA_Addresses")]
    public class DA_AddressesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_Addresses")]
        public long Id { get; set; }

        /// <summary>
        /// CustomerId or StoreId
        /// </summary>
        [Column("OwnerId", Order = 2,TypeName = "INT")]
        [Required]
        public int OwnerId { get; set; }

        [Column("AddressStreet", Order = 3, TypeName = "NVARCHAR(200)")]
        [Required]
        public string AddressStreet { get; set; }

        [Column("AddressNo", Order = 4, TypeName = "NVARCHAR(20)")]
        [Required]
        public string AddressNo { get; set; }

        /// <summary>
        /// κάθετος δρόμος
        /// </summary>
        [Column("VerticalStreet", Order = 5, TypeName = "NVARCHAR(200)")]
        public string VerticalStreet { get; set; }

        [Column("Floor", Order = 6, TypeName = "NVARCHAR(200)")]
        public string Floor { get; set; }

        [Column("Area", Order = 7, TypeName = "NVARCHAR(100)")]
        public string Area { get; set; }

        [Column("City", Order = 8, TypeName = "NVARCHAR(100)")]
        [Required]
        public string City { get; set; }

        [Column("Zipcode", Order = 9, TypeName = "NVARCHAR(10)")]
        public string Zipcode { get; set; }

        [Column("Latitude", Order = 10, TypeName = "FLOAT")]
        [Required]
        public float Latitude { get; set; }

        [Column("Longtitude", Order = 11, TypeName = "FLOAT")]
        [Required]
        public float Longtitude { get; set; }

        /// <summary>
        /// σχόλια (εμφάνιση στο ΔΠ)
        /// </summary>
        [Column("Notes", Order = 12, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }

        /// <summary>
        /// περιγραφή κουδουνιού (εμφάνιση στο ΔΠ)
        /// </summary>
        [Column("Bell", Order = 13, TypeName = "NVARCHAR(200)")]
        public string Bell { get; set; }

        /// <summary>
        /// 0:Shipping, 1: Billing, 2: Store
        /// </summary>
        [Column("AddressType", Order = 14, TypeName = "INT")]
        [Required]
        public int AddressType { get; set; }

        /// <summary>
        /// json object
        /// </summary>
        [Column("ExtObj", Order = 15, TypeName = "TEXT")]
        public string ExtObj { get; set; }

        /// <summary>
        /// Extrernal Id (e-food κτλ)
        /// </summary>
        [Column("ExtId1", Order = 16, TypeName = "NVARCHAR(50)")]
        public string ExtId1 { get; set; }

        /// <summary>
        /// Extrernal Id (e-food κτλ)
        /// </summary>
        [Column("ExtId2", Order = 17, TypeName = "NVARCHAR(50)")]
        public string ExtId2 { get; set; }

        /// <summary>
        /// Φιλική ονομασία διεύθυνσης
        /// </summary>
        [Column("FriendlyName", Order = 18, TypeName = "NVARCHAR(200)")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Describes Address Proximity 0 (the worst Result) to 3 (the best result).
        /// 0:APPROXIMATE, 1: GEOMETRIC_CENTER, 2: RANGE_INTERPOLATED, 3: ROOFTOP
        /// </summary>
        [Column("AddressProximity", Order = 19, TypeName = "INT")]
        public int AddressProximity { get; set; }

        /// <summary>
        /// Address is deleted
        /// </summary>
        [Column("IsDeleted", Order = 20, TypeName = "BIT")]
        [Required]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Last phone number associated with this address
        /// </summary>
        [Column("LastPhoneNumber", Order = 21, TypeName = "NVARCHAR(20)")]
        public string LastPhoneNumber { get; set; }


        /// <summary>
        /// Phonetic words for address
        /// </summary>
        [Column("Phonetics", Order = 22, TypeName = "NVARCHAR(1000)")]
        public string Phonetics { get; set; }

        /// <summary>
        /// Phonetic words for Area
        /// </summary>
        [Column("PhoneticsArea", Order = 23, TypeName = "NVARCHAR(1000)")]
        public string PhoneticsArea { get; set; }


        /// <summary>
        /// Geography Column for Custimer Addresses
        /// </summary>
        //[Column("geographyColumn", Order = 20, TypeName = "geography")]
        //public SqlGeography geographyColumn { get; set; }

    }
}
