using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.ExternalSystems
{
    public class ICouponToken
    {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        //public DateTime issued { get; set; }
        //public DateTime expires { get; set; }
    }

    /// <summary>
    /// Model returned from get coupons as list callback used and created for deseriallization reasons
    /// </summary>
    public class CouponsResponse
    {
        public List<ICoupon> coupons { get; set; }
    }

    /// <summary>
    /// Main iCoupon model used to Get and Choose Discounts and also the Model to Post on update callback
    /// of coupon use or manage
    /// </summary>
    public class ICoupon
    {
        /// <summary>
        /// The reference of the voucher    
        /// </summary>
        public string couponRef { get; set; }

        /// <summary>
        /// Whether the voucher has been redeemed or not 
        /// </summary>
        public bool redeemed { get; set; }

        /// <summary>
        ///  Whether the voucher has expired or not
        /// </summary>
        public bool expired { get; set; }

        /// <summary>
        /// The description of the voucher  
        /// </summary>
        public string couponTypeName { get; set; }

        /// <summary>
        /// The value of the voucher  
        /// </summary>
        public decimal value { get; set; }

        /// <summary>
        /// The currency code of the voucher 
        /// </summary>
        public string currencyCode { get; set; }

        /// <summary>
        /// The currency symbol of the voucher  
        /// </summary>
        public string currencySymbol { get; set; }

        /// <summary>
        /// The date and time that the status of the voucher last changed.  
        /// This will be the issue time unless the voucher has already been redeemed when it will be the redeemed time.	date time
        /// </summary>
        public DateTime statusChangeDate { get; set; }

        /// <summary>
        /// A POS specific object reference used to specify how the voucher should be handled by the POS.  	
        /// </summary>
        public string objectRef { get; set; }

        /// <summary>
        /// The type of the voucher Possible values:
        /// •	1 – discount
        /// •	2 – tender
        /// </summary>
        public int type { get; set; }
    }

    /// <summary>
    /// Model used as form post to get coupons use this to create dictionary to post on icoupon API
    /// </summary>
    public class ICouponGetCouponFormModel
    {

        /// <summary>
        /// The barcode to find vouchers for	body string
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// The format of the barcode. This should normally be set to “auto” to allow for support of new barcode formats without changing the client code.body string
        /// Possible values:    •	auto – will detect which of the supported barcode formats was passed in
        ///                     •	iata – an IATA boarding pass
        /// </summary>
        public string barcodeFormat { get; set; }

        /// <summary>
        /// Till specific data. It would be very useful to include a check or receipt identifier in this field as it helps to resolve any issues that may occur.body    string
        /// </summary>
        public string tillRef { get; set; }

        /// <summary>
        /// The airport or site code body    string
        /// </summary>
        public string locationRef { get; set; }

        /// <summary>
        /// Retailer unique identifier body    string
        /// </summary>
        public string serviceProviderRef { get; set; }

        /// <summary>
        /// Unique identifier of the trading outlet body    string
        /// </summary>
        public string tradingOutletRef { get; set; }

        //public Dictionary<string, string> ToDictionary()
        //{
        //    return new Dictionary<string, string> {
        //        { "barcode", this.barcode},
        //        { "barcodeFormat", this.barcodeFormat},
        //        { "tillRef", this.tillRef },
        //        { "locationRef", this.locationRef },
        //        { "serviceProviderRef", this.serviceProviderRef },
        //        { "tradingOutletRef", this.tradingOutletRef },
        //    };
        //}
    }


    /// <summary>
    /// Model used to create Dictionary on post of a coupon
    /// </summary>
    public class ICouponUpdateFormModel
    {
        /// <summary>
        /// The reference of the voucher to update  path string 
        /// </summary>
        //public string couponRef { get; set; }

        /// <summary>
        /// The action to perform on the coupon body    string
        /// Possible values:    •	redeem – will redeem the voucher
        ///                     •	void – will void a voucher redemption allowing it to be redeemed again
        /// </summary>
        public string action { get; set; }


        /// <summary>
        /// Till specific data. It would be very useful to include a check or receipt identifier
        /// in this field as it helps to resolve any issues that may occur.body    string
        /// </summary>
        public string tillRef { get; set; }

        /// <summary>
        /// The airport or site code body    string
        /// </summary>
        public string locationRef { get; set; }

        /// <summary>
        /// Retailer unique identifier body    string
        /// </summary>
        public string serviceProviderRef { get; set; }

        /// <summary>
        /// Unique identifier of the trading outlet body    string
        /// </summary>
        public string tradingOutletRef { get; set; }

        /// <summary>
        /// The name of the trading outlet  body    string
        /// </summary>
        public string tradingOutletName { get; set; }

        //public Dictionary<string, string> ToDictionary()
        //{
        //    return new Dictionary<string, string> {
        //        //{ "couponRef", this.couponRef},
        //        { "action", this.action},
        //        { "tillRef", this.tillRef },
        //        { "locationRef", this.locationRef },
        //        { "serviceProviderRef", this.serviceProviderRef },
        //        { "tradingOutletRef", this.tradingOutletRef },
        //        { "tradingOutletName", this.tradingOutletName }
        //    };
        //}
    }



    public class Redemption
    {
        public string objectRef { get; set; }

        public string currencyCode { get; set; }
        public float value { get; set; }
        public int type { get; set; }
    }

    /// <summary>
    /// Model used to create Dictionary on post of 
    /// </summary>
    public class ICouponInserReceiptFormModel
    {
        /// <summary>
        /// Retailer unique identifier body    string
        /// </summary>
        public string serviceProviderRef { get; set; }

        /// <summary>
        /// Unique identifier of the trading outlet body    string
        /// </summary>
        public string tradingOutletRef { get; set; }

        /// <summary>
        /// The name of the trading outlet  body    string
        /// </summary>
        public string tradingOutletName { get; set; }

        /// <summary>
        /// Check / receipt number. This should match the receipt given to the customer.
        /// </summary>
        public string receiptNumber { get; set; }

        /// <summary>
        /// Check / receipt date. This should match the receipt given to the customer.
        /// </summary>
        public string receiptDate { get; set; }

        /// <summary>
        /// The redemption value of the coupon to 2 decimal places.
        /// </summary>
        public string redeemedValue { get; set; }

        /// <summary>
        /// An array of ReceiptLine records
        /// </summary>
        public List<ReceiptLines> receiptLines { get; set; }

        /// <summary>
        /// An array of GlLine records
        /// </summary>
        public List<GlLines> glLines { get; set; }

        //public Dictionary<string, string> ToDictionary()
        //{
        //    string s = string.Format("{0:0.00}", this.redeemedValue);
        //    s = s.Replace(",",".");
        //    return new Dictionary<string, string> {
        //        { "serviceProviderRef", this.serviceProviderRef },
        //        { "tradingOutletRef", this.tradingOutletRef },
        //        { "tradingOutletName", this.tradingOutletName },
        //        { "receiptNumber", this.receiptNumber },
        //        { "receiptDate", this.receiptDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff") },
        //        { "redeemedValue", s },
        //        { "receiptLines", JsonConvert.SerializeObject(this.receiptLines) },
        //        { "glLines", JsonConvert.SerializeObject(this.glLines) }
        //    };
        //}
    }

    /// <summary>
    /// Model used to Provide sales data for a redeemed coupon 
    /// </summary>
    public class IcouponInsertReceiptsDataModel
    {
        /// <summary>
        /// The reference of the voucher    
        /// </summary>
        public string couponRef { get; set; }

        /// <summary>
        /// Retailer unique identifier
        /// </summary>
        public string serviceProviderRef { get; set; }

        /// <summary>
        /// Unique identifier of the trading outlet
        /// </summary>
        public string tradingOutletRef { get; set; }

        /// <summary>
        /// The name of the trading outlet
        /// </summary>
        public string tradingOutletName { get; set; }

        /// <summary>
        /// Check / receipt number. This should match the receipt given to the customer.
        /// </summary>
        public string receiptNumber { get; set; }

        /// <summary>
        /// Check / receipt date. This should match the receipt given to the customer.
        /// </summary>
        public DateTime receiptDate { get; set; }

        /// <summary>
        /// The redemption value of the coupon to 2 decimal places.
        /// </summary>
        public decimal redeemedValue { get; set; }

        /// <summary>
        /// An array of ReceiptLine records
        /// </summary>
        public List<ReceiptLines> receiptLines { get; set; }

        /// <summary>
        /// An array of GlLine records
        /// </summary>
        public List<GlLines> glLines { get; set; }
    }

    /// <summary>
    /// The lines of the receipt
    /// </summary>
    public class ReceiptLines
    {
        /// <summary>
        /// The type of line  
        /// Possible values:
        ///    •	1 – product
        ///    •	2 – adjustment
        ///    •	4 – payment
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// The name of the product, adjustment or payment
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// The line quantity of product lines.  Set to 1 for adjustment and payment lines.  
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// The total value of the product sold.
        /// </summary>
        public decimal value { get; set; }

        /// <summary>
        /// Whether this receipt line is the one that represents the voucher used.
        /// </summary>
        public bool isCoupon { get; set; }
    }

    /// <summary>
    /// The GL lines of the receipt
    /// </summary>
    public class GlLines
    {
        /// <summary>
        /// The general ledger accounting code.  Can be any identifier for the product or tender.
        /// </summary>
        public string gl { get; set; }

        /// <summary>
        /// The product or tender name depending on coupon type.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Coupon value of product / tender rounded to 2 decimal places.
        /// </summary>
        public decimal value { get; set; }

        /// <summary>
        /// Tax value to 6 decimal places – only applies to product lines for discount type coupons.
        /// </summary>
        public decimal taxValue { get; set; }

        /// <summary>
        /// Percentage tax rate to 2 decimal places.
        /// </summary>
        public decimal taxRate { get; set; }
    }
}
