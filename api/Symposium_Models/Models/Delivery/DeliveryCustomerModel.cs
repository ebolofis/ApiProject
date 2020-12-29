using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class DeliveryCustomerLookupModel
    {
        public List<DeliveryCustomerTypeModel> CustomerType { get; set; }
        public List<DeliveryPhoneTypesModel> PhoneType { get; set; }
        public List<DeliveryAddressTypesModel> AddressType { get; set; }
        public List<PricelistModel> Pricelist { get; set; }
    }
    public class DeliveryCustomerFilterModel
    {
        //public int Page { get; set; }
        //public int PageSize { get; set; }
        public string Name { get; set; }
        public string VatNo { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
    public class DeliveryCustomer
    {
        public long ID { get; set; }
        //[Required]
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string VatNo { get; set; }
        public string DOY { get; set; }
        public string Floor { get; set; }
        public string email { get; set; }
        public string Comments { get; set; }
        public string BillingName { get; set; }
        public string BillingVatNo { get; set; }
        public string BillingDOY { get; set; }
        public string BillingJob { get; set; }
        public Nullable<int> CustomerType { get; set; } //Take id of entity and manage table  and assign to model CustomerType.Id
        public Nullable<long> DefaultPricelistId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ExtCustId { get; set; }
        public Nullable<int> ExtType { get; set; }
        public string ExtObj { get; set; }

        public Nullable<bool> GDPR_IsDeleted { get; set; }
        public Nullable<bool> GDPR_Marketing { get; set; }
        public Nullable<bool> GDPR_Returner { get; set; }
        public long GDPR_StaffId { get; set; }
        public string GDPR_StaffName { get; set; }
        public string PhoneComp { get; set; }
        public Nullable<bool> SendSms { get; set; }
        public Nullable<bool> SendEmail { get; set; }
        public string Proffesion { get; set; }
    }

    public class DeliveryCustomerModel : DeliveryCustomer
    {
        //Take id of entity and manage table and assign to model CustomerType.Id
        //Collections of  lookup entities mapped on DeliveryCustomersPhonesAndAddressModel by CustomerId = this.Id
        public List<DeliveryCustomersPhonesModel> Phones { get; set; }
        public List<DeliveryCustomersShippingAddressModel> ShippingAddresses { get; set; }
        public List<DeliveryCustomersBillingAddressModel> BillingAddresses { get; set; }
        public List<DeliveryCustomersPhonesAndAddressModel> Assocs { get; set; }
    }

    public class DeliveryCustomerModelDS : DeliveryCustomerModel
    {
        public long GuestId { get; set; }
    }
    public class DeliveryCustomerSearchModel : DeliveryCustomer
    {
        public string AA { get; set; }
        public string Name { get; set; }
        public Nullable<long> PhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<long> ShippingAddressId { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingNo { get; set; }
        public string ShippingCity { get; set; }

        public Nullable<long> BillingAddressId { get; set; }
        public string BillingAddress { get; set; }
        public string BillingNo { get; set; }
        public string BillingCity { get; set; }

    }


    //***************************************************************************** Assoc
    public class DeliveryCustomersPhonesAndAddressModel
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        //[Required]
        public long PhoneID { get; set; }
        //[Required]
        public long AddressID { get; set; }
        //[Required]
        public short IsShipping { get; set; }
    }

    //**************************************************************************** Phones

    public class DeliveryCustomersPhonesModel
    {
        public long ID { get; set; }
        //[Required]
        public long CustomerID { get; set; }
        //[Required]
        public string PhoneNumber { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        //[Required]
        public Nullable<int> PhoneType { get; set; }
        public string ExtKey { get; set; }
        public Nullable<int> ExtType { get; set; }
        public string ExtObj { get; set; }
    }


    //**************************************************************************** Addresses
    public class DeliveryCustomersShippingAddressModel
    {
        public long ID { get; set; }
        //[Required]
        public long CustomerID { get; set; }
        //[Required]
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public string SpecificIndication { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string Floor { get; set; }
        public bool IsSelected { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> Type { get; set; }
        public string ExtKey { get; set; }
        public Nullable<int> ExtType { get; set; }
        public string ExtObj { get; set; }
        public string ExtId1 { get; set; }
        public string ExtId2 { get; set; }
    }

    public class DeliveryCustomersBillingAddressModel
    {
        public long ID { get; set; }
        //[Required]
        public long CustomerID { get; set; }
        //[Required]
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public string SpecificIndication { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string Floor { get; set; }
        public bool IsSelected { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> Type { get; set; }
        public string ExtKey { get; set; }
        public Nullable<int> ExtType { get; set; }
        public string ExtObj { get; set; }
        public string ExtId1 { get; set; }
        public string ExtId2 { get; set; }
    }

    //**************************************************************************** Types
    public class DeliveryCustomerTypeModel
    {
        public int ID { get; set; }
        //[Required]
        public string Descr { get; set; }
    }

    public class DeliveryPhoneTypesModel
    {
        public int ID { get; set; }
        //[Required]
        public string Descr { get; set; }
    }

    public class DeliveryAddressTypesModel
    {
        public int ID { get; set; }
        //[Required]
        public string Descr { get; set; }
    }

}
