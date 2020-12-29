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
    [DisplayName("DA_Customers")]
    [Table("DA_Customers")]
    public class DA_CustomersDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_Customers")]
        public long Id { get; set; }

        /// <summary>
        /// Email/username
        /// </summary>
        [Column("Email", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Email { get; set; }

        [Column("Password", Order = 3, TypeName = "NVARCHAR(500)")]
        public string Password { get; set; }

        /// <summary>
        /// όνομα
        /// </summary>
        [Column("FirstName", Order = 4, TypeName = "NVARCHAR(100)")]
        public string FirstName { get; set; }

        /// <summary>
        /// επόνυμο
        /// </summary>
        [Column("LastName", Order = 5, TypeName = "NVARCHAR(100)")]
        [Required]
        public string LastName { get; set; }

        [Column("Phone1", Order = 6, TypeName = "NVARCHAR(20)")]
        public string Phone1 { get; set; }

        [Column("Phone2", Order = 7, TypeName = "NVARCHAR(20)")]
        public string Phone2 { get; set; }

        /// <summary>
        /// Κινητό τηλ
        /// </summary>
        [Column("Mobile", Order = 8, TypeName = "NVARCHAR(20)")]
        public string Mobile { get; set; }

        /// <summary>
        /// Διεύθυνση επιχείρησης  (τιμολόγιο) DA_Addresses.Id
        /// </summary>
        [Column("BillingAddressesId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> BillingAddressesId { get; set; }

        /// <summary>
        /// ΑΦΜ (τιμολόγιο)
        /// </summary>
        [Column("VatNo", Order = 10, TypeName = "NVARCHAR(20)")]
        public string VatNo { get; set; }

        /// <summary>
        /// ΔΟΥ (τιμολόγιο)
        /// </summary>
        [Column("Doy", Order = 11, TypeName = "NVARCHAR(100)")]
        public string Doy { get; set; }

        /// <summary>
        /// Επωνυμία επιχείρησης (τιμολόγιο)
        /// </summary>
        [Column("JobName", Order = 12, TypeName = "NVARCHAR(200)")]
        public string JobName { get; set; }

        /// <summary>
        /// Διεύθυνση τελευταίας παραγγελίας
        /// </summary>
        [Column("LastAddressId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> LastAddressId { get; set; }

        /// <summary>
        /// σχόλια από πελάτη (εμφάνιση στο ΔΠ)
        /// </summary>
        [Column("Notes", Order = 14, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }

        /// <summary>
        /// κρυφά σχόλια για τον πελάτη από τον υπάλληλο
        /// </summary>
        [Column("SecretNotes", Order = 15, TypeName = "NVARCHAR(1500)")]
        public string SecretNotes { get; set; }

        /// <summary>
        /// Σχόλιο Τελευταίας Παραγγελίας
        /// </summary>
        [Column("LastOrderNote", Order = 16, TypeName = "NVARCHAR(1500)")]
        public string LastOrderNote { get; set; }


        [Column("GTPR_Marketing", Order = 17, TypeName = "BIT")]
        public Nullable<bool> GTPR_Marketing { get; set; }

        [Column("GTPR_Returner", Order = 18, TypeName = "BIT")]
        public Nullable<bool> GTPR_Returner { get; set; }

        /// <summary>
        /// true: ο χρήστης έχει loyalty
        /// </summary>
        [Column("Loyalty", Order = 19, TypeName = "BIT")]
        public Nullable<bool> Loyalty { get; set; }

        /// <summary>
        /// Session Key for password change
        /// </summary>
        [Column("SessionKey", Order = 20, TypeName = "NVARCHAR(200)")]
        public string SessionKey { get; set; }

        /// <summary>
        /// Id from external systems (ex: efood etc)
        /// </summary>
        [Column("ExtId1", Order = 21, TypeName = "NVARCHAR(50)")]
        public string ExtId1 { get; set; }

        /// <summary>
        /// Id from external systems (ex: efood etc)
        /// </summary>
        [Column("ExtId2", Order = 22, TypeName = "NVARCHAR(50)")]
        public string ExtId2 { get; set; }

        /// <summary>
        /// Id from external systems (ex: efood etc)
        /// </summary>
        [Column("ExtId3", Order = 23, TypeName = "NVARCHAR(50)")]
        public string ExtId3 { get; set; }
        /// <summary>
        /// Id from external systems (ex: efood etc)
        /// </summary>
        [Column("ExtId4", Order = 23, TypeName = "NVARCHAR(50)")]
        public string ExtId4 { get; set; }

        /// <summary>
        /// Customer is deleted
        /// </summary>
        [Column("IsDeleted", Order = 24, TypeName = "BIT")]
        [Required]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Company Phone Number
        /// </summary>
        [Column("PhoneComp", Order = 25, TypeName = "NVARCHAR(20)")]
        public string PhoneComp { get; set; }

        /// <summary>
        /// true:Send SMS to customer
        /// </summary>
        [Column("SendSms", Order = 26, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_Customers_SendSms", NullDisplayText = "'False'")]
        public bool SendSms { get; set; }

        /// <summary>
        /// true:Send email's to customer
        /// </summary>
        [Column("SendEmail", Order = 27, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_DA_Customers_SendEmail", NullDisplayText = "'False'")]
        public bool SendEmail { get; set; }

        /// <summary>
        /// Proffesion name of customer
        /// </summary>
        [Column("Proffesion", Order = 28, TypeName = "NVARCHAR(100)")]
        public string Proffesion { get; set; }

        
        /// <summary>
        /// old Email
        /// </summary>
        [Column("EmailOld", Order = 29, TypeName = "NVARCHAR(50)")]
        public string EmailOld { get; set; }

        /// <summary>
        /// Authentication Token. A DA_Customer must authenticate himself either using 'username:password' or 'AuthToken'. (see BasicAuthHttpModule)
        /// </summary>
        [Column("AuthToken", Order = 30, TypeName = "NVARCHAR(500)")]
        public string AuthToken { get; set; }

        /// <summary>
        ///  Ημερομηνία Δημιουργίας
        /// </summary>
        [Column("CreateDate", Order = 31, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreateDate { get; set; }

        /// <summary>
        /// Metadata for web
        /// </summary>
        [Column("Metadata", Order = 32, TypeName = "NVARCHAR(MAX)")]
        public string Metadata { get; set; }

        /// <summary>
        /// IsAnonymous enumerator 0: I am not anonymous, 1: I will be anonymous, 2: I am anonymous
        /// </summary>
        [Column("IsAnonymous", Order = 33, TypeName = "INT")]
        public Nullable<DA_CustomerAnonymousTypeEnum> IsAnonymous { get; set; } = DA_CustomerAnonymousTypeEnum.IsNotAnonymous;

    }
}
