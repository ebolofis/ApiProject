using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.ExternalSystems
{
    /// <summary>
    /// Order Model from External System
    /// </summary>
    public class GoodysDA_OrderModel
    {

        /// <summary>
        /// after order insert set the order no. (order no or Invoice No?)
        /// </summary>
        public int orderno { get; set; }

        /// <summary>
        /// Shop id da_Stores.code
        /// </summary>
        public string shopId { get; set; }

        /// <summary>
        /// Shop description
        /// </summary>
        public string shop { get; set; }

        /// <summary>
        /// Software send order
        /// </summary>
        public string software { get; set; }

        /// <summary>
        /// Order Received date and time
        /// </summary>
        public DateTime statusTime { get; set; }

        /// <summary>
        /// Contains comments for Loyalty
        /// </summary>
        public string room { get; set; }

        /// <summary>
        /// Type of payment 
        /// AP => invoice type 1 (Receipt)
        /// TM=> Imvoice type 7 (Invoice)
        /// </summary>
        public string payment { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string comments { get; set; }

        /// <summary>
        /// Total records of order detail
        /// </summary>
        public int mqty { get; set; }

        /// <summary>
        /// Order Received time
        /// </summary>
        public DateTime deliveryTime { get; set; }

        /// <summary>
        /// Not used at the moment
        /// </summary>
        public string takeAway { get; set; }

        /// <summary>
        /// List of order details
        /// </summary>
        public List<GoodysDA_OrderProductModel> products { get; set; }

        /// <summary>
        /// Order's customer
        /// </summary>
        public GoodysDA_OrderCustomerModel customer { get; set; }
    }

    /// <summary>
    /// Product for da order
    /// </summary>
    public class GoodysDA_OrderProductModel
    {
        /// <summary>
        /// product code
        /// </summary>
        public string item_code { get; set; }

        /// <summary>
        /// product description
        /// </summary>
        public string item_descr { get; set; }

        /// <summary>
        /// product vat code 
        /// </summary>
        public int item_vat { get; set; }

        /// <summary>
        /// product quantity reguired
        /// </summary>
        public int qty { get; set; }

        /// <summary>
        /// product amount reguired (for da order divide by 100)
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// product total amount reguired (for da order divide by 100)
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 0=> Main product
        /// 1=> Ingredient
        /// </summary>
        public bool isCombined { get; set; }

        /// <summary>
        /// product discount percentace
        /// </summary>
        public int? discount_per { get; set; }

        /// <summary>
        /// product discount amount (for da order divide by 100)
        /// </summary>
        public int? discount_amount { get; set; }

        /// <summary>
        /// product total discount (for da order divide by 100)
        /// </summary>
        public int? total_disc { get; set; }
        
    }

    /// <summary>
    /// External Customer model for DA_Order
    /// </summary>
    public class GoodysDA_OrderCustomerModel
    {

        /// <summary>
        /// After post order returns DA_Customer.Id
        /// </summary>
        public string customerid { get; set; }

        /// <summary>
        /// same as shopId for post order return
        /// </summary>
        public string store { get; set; }

        /// <summary>
        /// Extrnal system Customer Id Saved on DA_Customer.Ext1 field
        /// </summary>
        public string accountId { get; set; }

        /// <summary>
        /// Customer last name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Customer first name
        /// </summary>
        public string fname { get; set; }

        /// <summary>
        /// customer phone
        /// </summary>
        public string tel1 { get; set; }

        /// <summary>
        /// Customr's Address
        /// </summary>
        public string address1 { get; set; }

        /// <summary>
        /// Customer's address no
        /// </summary>
        public string addressNo { get; set; }

        /// <summary>
        /// Customer's floor
        /// </summary>
        public string orofos1 { get; set; }

        /// <summary>
        /// Customer's city
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// Customer's post code
        /// </summary>
        public string zipcode { get; set; }

        /// <summary>
        /// Customer's email
        /// </summary>
        [EmailAddress]
        public string email { get; set; }

        /// <summary>
        /// Customer's address id from External System. Saved on DA_Addresses.Ext1 field
        /// </summary>
        public string addressId { get; set; }

        /// <summary>
        /// Customer's Doy for Invoice
        /// </summary>
        public string doy { get; set; }

        /// <summary>
        /// Customer's AFM for Invoice
        /// </summary>
        public string afm { get; set; }

        /// <summary>
        /// Customer's company name for Invoice ????
        /// </summary>
        public string company_name { get; set; }

        /// <summary>
        /// Customer's billing address
        /// </summary>
        public string bl_address { get; set; }

        /// <summary>
        /// Customer's billing address no
        /// </summary>
        public string bl_address_no { get; set; }

        /// <summary>
        /// Customer's billing city
        /// </summary>
        public string bl_city { get; set; }

        /// <summary>
        /// Customer's profession for invoice
        /// </summary>
        public string profession { get; set; }
    }

    /// <summary>
    /// Responce customer model after login
    /// </summary>
    public class GoodysLoginResponceModel
    {
        /// <summary>
        /// DA_Customer id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// if not deleted then Active
        /// </summary>
        public string accountStatus { get; set; }

        /// <summary>
        /// Phone ???? (phone on example)
        /// </summary>
        public string location { get; set; }

        /// <summary>
        /// Login name (email on example)
        /// </summary>
        [EmailAddress]
        public string name { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string lastName { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string firstName { get; set; }

        /// <summary>
        /// ??? (web on eample)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// List of addresses
        /// </summary>
        public List<GoodysLoginAddressResponceModel> addressList { get; set; }
    }

    /// <summary>
    /// Responce address model for customer
    /// </summary>
    public class GoodysLoginAddressResponceModel
    {
        /// <summary>
        /// Comments
        /// </summary>
        public string addressComment { get; set; }

        /// <summary>
        /// DA_Addresses.Id
        /// </summary>
        public string addressId { get; set; }

        /// <summary>
        /// Alias for address (Friendly name) (email_Home on example)
        /// </summary>
        public string addressNAme { get; set; }

        /// <summary>
        /// Floor
        /// </summary>
        public string addresssFloor { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string county { get; set; }

        /// <summary>
        /// Address number
        /// </summary>
        public string NNHome { get; set; }

        /// <summary>
        /// Phone number from customer then first not null field(phone, phone1, mobile) 
        /// </summary>
        public string phoneNumber { get; set; }
        
        /// <summary>
        /// Post code
        /// </summary>
        public string postalCode { get; set; }

        /// <summary>
        /// null because we get shop on customer search to be soure polygons not chenged
        /// </summary>
        public string shop { get; set; }

        /// <summary>
        /// null because we get shop on customer search to be soure polygons not chenged
        /// </summary>
        public string shopId { get; set; }

        /// <summary>
        /// ???? (HIT on example)
        /// </summary>
        public string software { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public string specificAddrChar { get; set; }

        /// <summary>
        /// Area
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Address street name
        /// </summary>
        public string streetAddress { get; set; }

        /// <summary>
        /// Second Street alias (Friendly name)
        /// </summary>
        public string streetAlias { get; set; }

        /// <summary>
        /// id da_customer.lastaddresid is not 0 then this address as primary else the first one
        /// </summary>
        public string isPrimary { get; set; }

        /// <summary>
        /// Account Id (DA_Customer Ext1 or Id) for new or delete address
        /// </summary>
        public string accId { get; set; }

        public bool isShipping { get; set; }
    }

    /// <summary>
    /// Registration Model for Customer and Address
    /// </summary>
    public class GoodysRegisterModel
    {
        /// <summary>
        /// Area
        /// </summary>
        public string addrState { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string addrCounty { get; set; }

        /// <summary>
        /// ZipCode
        /// </summary>
        public string addrPostalCode { get; set; }

        /// <summary>
        /// Street Name
        /// </summary>
        public string addrStreetName { get; set; }

        /// <summary>
        /// Street no
        /// </summary>
        public string addrNumber { get; set; }

        /// <summary>
        /// Floor
        /// </summary>
        public string addrFloor { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string addrComments { get; set; }

        /// <summary>
        /// Friendly Name
        /// </summary>
        public string addrShortDesc { get; set; }

        /// <summary>
        /// Customer phone 2
        /// </summary>
        public string addrPhoneNum { get; set; }

        /// <summary>
        /// ?????
        /// </summary>
        public string addrSpecificChar { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string fName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string lName { get; set; }

        /// <summary>
        /// email
        /// </summary>
        [EmailAddress]
        public string acceMail { get; set; }

        /// <summary>
        /// Phone 1
        /// </summary>
        public string accountPhone { get; set; }
    }

    /// <summary>
    /// Reads property room from main model and gets all available coupons
    /// </summary>
    public class GetCouponsFromStringModel
    {
        /// <summary>
        /// If order is WEB or MOBILE
        /// </summary>
        public string WEB { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public string DE { get; set; }

        /// <summary>
        /// Coupon code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Coupon Campain
        /// </summary>
        public string CouponCamp { get; set; }

        /// <summary>
        /// Gift card code
        /// </summary>
        public string GiftCard { get; set; }

        /// <summary>
        /// Loyalty code
        /// </summary>
        public string Loyalty { get; set; }
    }
}
