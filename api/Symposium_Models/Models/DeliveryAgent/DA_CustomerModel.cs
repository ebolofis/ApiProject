
using Symposium.Models.CustomAttributes;
using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    /// <summary>
    ///  DACustomerModel  (Email and Password are NOT required)
    /// </summary>
    public class DACustomerModel : DACustomerBasicModel, IDA_CustomerModel
    {
        [MaxLength(50)]
        [MinLength(8)]
        public string Email { get; set; }

        [MaxLength(50)]
        [MinLength(5)]
        public string Password { get; set; }

        [MaxLength(1500)]
        public string SecretNotes { get; set; }
    }

    /// <summary>
    /// DACustomerModel: 
    ///   1. DA_AddressesModels are included,  
    ///   2. Email and Password are required
    ///   (mainly for web user but for others as well)
    /// </summary>
    public class DACustomerExtModel : DACustomerBasicModel, IDA_CustomerModel
    {
        [MaxLength(50)]
        [MinLength(8)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MaxLength(50)]
        [MinLength(5)]
        [Required]
        public string Password { get; set; }
        [Required]
       // [EnsureMinimumElements(1, ErrorMessage = "At least one Address is required")]
        public List<DA_AddressModel> ShippingAddresses { get; set; }

        public DA_AddressModel BillingAddress { get; set; }
    }


    public class DACustomerBasicModel
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [MinLength(4)]
        [Required]
        public string LastName { get; set; }
        [MaxLength(20)]
        [MinLength(10)]
        public string Phone1 { get; set; }
        [MaxLength(20)]
        [MinLength(10)]
        public string Phone2 { get; set; }
        [MaxLength(20)]
        [MinLength(10)]
        public string Mobile { get; set; }

        public long BillingAddressesId { get; set; }
        [MaxLength(20)]
        [MinLength(9)]
        public string VatNo { get; set; }
        [MaxLength(100)]
        [MinLength(4)]
        public string Doy { get; set; }
        [MaxLength(200)]
        [MinLength(2)]
        public string JobName { get; set; }

        public long LastAddressId { get; set; }
        [MaxLength(1500)]
        public string Notes { get; set; }

        [MaxLength(1500)]
        public string LastOrderNote { get; set; }

        public Nullable<bool> GTPR_Marketing { get; set; }
        public Nullable<bool> GTPR_Returner { get; set; }
        public Nullable<bool> Loyalty { get; set; }
        public string SessionKey { get; set; }
        public string ExtId1 { get; set; }
        public string ExtId2 { get; set; }
        public string ExtId3 { get; set; }
        public string ExtId4 { get; set; }
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Company Phone Number
        /// </summary>
        [MaxLength(20)]
        [MinLength(10)]
        public string PhoneComp { get; set; }

        /// <summary>
        /// true:Send SMS to customer
        /// </summary>
        [Required]
        public bool SendSms { get; set; }

        /// <summary>
        /// true:Send email's to customer
        /// </summary>
        [Required]
        public bool SendEmail { get; set; }

        /// <summary>
        /// Profession name of customer
        /// </summary>
        [MaxLength(100)]
        [MinLength(3)]
        public string Proffesion { get; set; }

        /// <summary>
        /// Type of calling...  etc. vivardia, forkey, DeliveryAgent, Default...
        /// </summary>
        public ExternalSystemOrderEnum ExtType { get; set; }

        /// <summary>
        /// old Email
        /// </summary>
        public string EmailOld { get; set; }

        /// <summary>
        /// Authentication Token. A DA_Customer must authenticate himself either using 'username:password' or 'AuthToken'. (see BasicAuthHttpModule)
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        ///  Ημερομηνία Δημιουργίας
        /// </summary>
        public Nullable<DateTime> CreateDate { get; set; }

        /// <summary>
        /// Metadata for web
        /// </summary>
        [MaxLength(4000)]
        public string Metadata { get; set; }

        public Nullable<DA_CustomerAnonymousTypeEnum> IsAnonymous { get; set; } = DA_CustomerAnonymousTypeEnum.IsNotAnonymous;

        public override string ToString()
        {
            string customerInfo = Id.ToString() + ": " + (FirstName ?? "") + " " + (LastName ?? "") + ", " + (Mobile ?? "") + ", " + (Phone1 ?? "") + ", " + (Phone2 ?? "");
            return customerInfo;
        }

    }


    /// <summary>
    /// Model for logging in using email/password
    /// </summary>
    public class DALoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Model for logging in using email/password, login datetime
    /// </summary>
    public class DALoginModelExt : DALoginModel
    {
        [Required]
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// Model for logging in using email/SessionKey
    /// </summary>
    public class DALoginSessionKeyModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string SessionKey { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        public string Email { get; set; }
    }

    /// <summary>
    /// Model for changing password
    /// </summary>
    public class DAchangePasswordModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Model for reseting password
    /// </summary>
    public class DAresetPasswordModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class GetExternalIdModel
    {
        [Required]
        public string Email { get; set; }
    }

    public class ExternalId2PasswordModel
    {
        public string ExtId2 { get; set; }
        public bool hasPassword { get; set; }
    }

    public class DASearchCustomerModel
    {
        public DACustomerModel daCustomerModel { get; set; }
        public List<DA_AddressModel> daAddrModel { get; set; }

        public int ExtType { get; set; }

        /// <summary>
        /// 20=delivary, 21=takeout
        /// On takeout no address exists
        /// </summary>
        public Nullable<Int16> OrderType { get; set; }

        public Nullable<long> CustomerId { get; set; }

        public Nullable<long> OrderId { get; set; }
    }

    public class DACustomerOrderModel : DACustomerModel
    {
        public List<DA_AddressModel> daAddrModel { get; set; }

        public int ExtType { get; set; }

        /// <summary>
        /// 20=delivary, 21=takeout
        /// On takeout no address exists
        /// </summary>
        public Nullable<Int16> OrderType { get; set; }

        public Nullable<long> OrderId { get; set; }

    }

    public class DACustomerIdentifyModel
    {

        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
    }

    public class DACustomerIdentifyResultModel
    {
        /// <summary>
        /// The Customer Id. In case of no customer found, CustomerId=0.
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        ///  3 = customer found with specific/not null values for email and mobile. 
        ///  2 = customer found with specific email and empty mobile. 
        ///  1 = customer found with specific mobile and empty email. 
        ///  0 = NO customer found  with specific/not null values for email and mobile.  
        /// </summary>
        public int Result { get; set; }
    }

    /// <summary>
    /// class used to cashed logins. see CashedLoginsHelper
    /// </summary>
    public class CashedLoginModel
    {
        /// <summary>
        /// Customer/Staff Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// string that contains 'username:password' or 'AuthToken'
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// last time the login occured
        /// </summary>
        public DateTime LastUpdate { get; set; }
    }

    public interface IDA_CustomerModel
    {
        long Id { get; set; }
        string Email { get; set; }

        string Password { get; set; }

        string FirstName { get; set; }
        string LastName { get; set; }
        string Phone1 { get; set; }
        string Phone2 { get; set; }
        string Mobile { get; set; }

        long BillingAddressesId { get; set; }
        string VatNo { get; set; }
        string Doy { get; set; }
        string JobName { get; set; }

        long LastAddressId { get; set; }

        string Notes { get; set; }

        string LastOrderNote { get; set; }

        Nullable<bool> GTPR_Marketing { get; set; }
        Nullable<bool> GTPR_Returner { get; set; }
        Nullable<bool> Loyalty { get; set; }
        string SessionKey { get; set; }
        string ExtId1 { get; set; }
        string ExtId2 { get; set; }
        bool IsDeleted { get; set; }
    }

    public class DACustomerSearchFilters
    {
        public DA_CustomerSearchTypeEnum Type { get; set; }

        public string Value { get; set; }
    }
}
