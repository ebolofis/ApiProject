using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{

    public class DA_OrderIdsModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Id from external system like e-food
        /// </summary>
        [MaxLength(50)]
        public string ExtId1 { get; set; }

        /// <summary>
        /// Id from external system like e-food
        /// </summary>
        [MaxLength(50)]
        public string ExtId2 { get; set; }

        /// <summary>
        /// ημερομινία/ώρα δημιουργίας παραγγελίας
        /// </summary>
        [Required]
        public DateTime OrderDate { get; set; }


        /// <summary>
        /// time of last search into the cashed list. Not inserted to DB
        /// </summary>
        public DateTime LastSearch { get; set; }
    }


    //-----------------------------------------------------
    public class DA_OrderModel : DA_OrderIdsModel
    {


        /// <summary>
        /// DA_Customers.Id
        /// </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        /// <summary>
        /// DA_Stores.Id (id καταστήματος). 
        /// One of StoreId and StoreCode should have value
        /// </summary>
        [Range(0, long.MaxValue)]
        public long StoreId { get; set; } = 0;

        /// <summary>
        /// DA_Stores.Code (Code καταστήματος). 
        /// One of StoreId and StoreCode should have value
        /// </summary>
        [StringLength(50)]
        public string StoreCode { get; set; }

        /// <summary>
        /// DA_GeoPolygons.Id
        /// </summary>
        [Range(0, long.MaxValue)]
        public long GeoPolygonId { get; set; }

        /// <summary>
        /// DA_Addresses.Id
        /// </summary>
        [Range(0, long.MaxValue)]
        public long? ShippingAddressId { get; set; } = null;



        /// <summary>
        /// εκτιμώμενη ημερομηνία/ώρα παράδωσης παραγγελίας
        /// </summary>
        public Nullable<DateTime> EstBillingDate { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα παράδωσης παραγγελίας
        /// </summary>
        public Nullable<DateTime> BillingDate { get; set; }

        /// <summary>
        /// εκτιμώμενη ημερομηνία/ώρα ΠΑΡΑΣΚΕΥΗΣ παραγγελίας
        /// </summary>
        public Nullable<DateTime> EstTakeoutDate { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα ΠΑΡΑΣΚΕΥΗΣ παραγγελίας
        /// </summary>
        public Nullable<DateTime> TakeoutDate { get; set; }

        /// <summary>
        /// ημερομηνια/ώρα που η παραγγελία έκλεισε (status: 4,5,6)
        /// </summary>
        public Nullable<DateTime> FinishDate { get; set; }

        /// <summary>
        /// διάρκεια παραγγελίας (εφόσον έχει κλείσει)
        /// </summary>
        public Nullable<int> Duration { get; set; }

        /// <summary>
        /// Διεύθυνση επιχείρησης  (τιμολόγιο)
        /// </summary>
        public Nullable<long> BillingAddressId { get; set; }

        /// <summary>
        /// Συνολικό ποσό (πριν την έκπτωση)
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Ποσό έκπτωσης
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Discount { get; set; }

        /// <summary>
        /// Συνολικό ποσό (μετά την έκπτωση)
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Total { get; set; }

        /// <summary>
        /// Συνολικό ΦΠΑ 
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// Συνολικός Δημ. Φόρος 
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// 1=cash, credit/debit card=4, voucher=6
        /// </summary>
        [Required]
        [Range(1, 10)]
        public Int16 AccountType { get; set; }

        /// <summary>
        /// 1=απόδειξη, 7=τιμολόγιο
        /// </summary>
        [Required]
        [Range(1, 10)]
        public Int16 InvoiceType { get; set; }

        /// <summary>
        /// 20=delivery, 21=takeout, 22=dinein
        /// On takeout no address exists
        /// </summary>
        [Required]
        [Range(20, 22)]
        public OrderTypeStatus OrderType { get; set; }

        /// <summary>
        /// Κατάσταση παραγγελίας
        /// </summary>
        [Required]
        [Range(0, 11)]
        public OrderStatusEnum Status { get; set; }

        /// <summary>
        /// ημερομηνία/ώρα αλλαγής status
        /// </summary>
        public DateTime StatusChange { get; set; } = DateTime.Now;

        /// <summary>
        /// Σχόλια/παράπονα για τη παραγγελία (απο πελάτη κτλ)
        /// </summary>
        [MaxLength(1500)]
        public string Remarks { get; set; }

        /// <summary>
        /// Order.Id που έδωσε το κατάστημα
        /// </summary>
        public long StoreOrderId { get; set; }

        /// <summary>
        /// 0 αν το κατάστημα έχει ενημερωθεί για την παραγγελία
        /// </summary>
        public Int16 IsSend { get; set; }

        /// <summary>
        /// προέλευση παραγγ.: 0: agent, 1: website/xmlFiles, 2: mobile, 3: e-food, 4: click delivery, 5: deliveras, 6: skroutz, 7: dine in app
        /// </summary>
        [Required]
        [Range(0, 7)]
        public Int16 Origin { get; set; }

        /// <summary>
        /// ExtType ex 1 Goodys Delivery....
        /// </summary>
        public int ExtType { get; set; }

        /// <summary>
        /// json object
        /// </summary>
        public string ExtObj { get; set; }



        /// <summary>
        /// Price-Discount-TotalVat-TotalTax
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// true: at least one item has changed
        /// </summary>
        public bool ItemsChanged { get; set; }

        [MaxLength(1500)]
        public string LastOrderNote { get; set; }

        //  public int RedeemLoyaltyPoints { get; set; }
        /// <summary>
        /// true: Order is paid
        /// </summary>
        [Required]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Order No from Client Store
        /// </summary>
        public Nullable<long> StoreOrderNo { get; set; }

        /// <summary>
        /// Point's gained by order (Loyalty)
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public int PointsGain { get; set; } = 0;

        /// <summary>
        /// Point's redeemed by order (Loyalty)
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public int PointsRedeem { get; set; } = 0;

        [Required]
        public List<DA_OrderDetails> Details { get; set; }

        /// <summary>
        /// PosInfo Id for Store. PosInfo.Id
        /// </summary>
        public int PosId { get; set; }
        /// <summary>
        /// true: παραγγελία με χρονοκαθηστέριση.
        /// </summary>
        public bool IsDelay { get; set; }

        /// <summary>
        /// Cover for order and invoice
        /// </summary>
        public Nullable<int> Cover { get; set; } = 0;



        /// <summary>
        /// Error Message if order not post to store
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Pos Info Detail for store, Update on Order flow
        /// </summary>
        public Nullable<long> StorePosInfoDetail { get; set; }

        /// <summary>
        /// Order Data from external system, ex: e-food
        /// </summary>
        public string ExtData { get; set; }

        /// <summary>
        /// Logical Error message etc. prices not same, different vats ....
        /// </summary>
        public string LogicErrors { get; set; }

        /// <summary>
        /// Discount Remparks. Sended to Invoice Table on Store and on Field DiscountRemark
        /// </summary>
        public string DiscountRemark { get; set; }

        /// <summary>
        /// if true then ignore Shortages at insert or update to DB
        /// </summary>
        public bool IgnoreShortages { get; set; } = false;

        /// <summary>
        /// DA Agent Staff Id
        /// </summary>
        public Nullable<long> Staffid { get; set; }

        /// <summary>
        /// DA Agent Order Number
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// External id of payment of bank transaction
        /// </summary>
        public string PaymentId { get; set; }

        [MaxLength(200)]
        public string LoyaltyCode { get; set; }
        [MaxLength(30)]
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Metadata for web
        /// </summary>
        [MaxLength(4000)]
        public string Metadata { get; set; }

        /// <summary>
        /// true: θα προστήθονται πόντοι loyalty στον πελάτη. This field is NOT inserted in DB.
        /// </summary>
        public bool AddLoyaltyPoints { get; set; } = true;

        /// <summary>
        /// true: ελέγχει τη διεύθυνση όταν εισάγεται μια παραγγελία από το bucket. This field is NOT inserted in DB.
        /// </summary>
        public bool CheckShippingAddress { get; set; } = true;

        /// <summary>
        /// true: το αρχικό κατάστημα της παραγγελίας όπως ορίζεται από την επιλεγμένη διεύθυνση είναι διαφορετικό από το επιλεγμένο κατάστημα. This field is NOT inserted in DB.
        /// </summary>
        public bool HasDifferentStore { get; set; }

        public Nullable<long> OrderNo { get; set; }

        public Nullable<long> TableId { get; set; }

        public DACustomerModel AnonymousCustomer {get; set;}

        public override string ToString()
        {
            string orderInfo = Id.ToString() + ": " + (OrderDate != null ? OrderDate.ToString() : "") + ", " + Price.ToString() + " -" + Discount.ToString();
            return orderInfo;
        }

    }

    // ---------------------------------------------------------------------

    public class DA_OrderExtDeliveryModel : DA_OrderModel
    {
        /// <summary>
        /// Errors from external delivery system like e-food. This field is NOT inserted to DB.
        /// </summary>
        public string ExtDeliveryErrors { get; set; } = "";

        /// <summary>
        /// flag for checking if order for external delivery system is already inserted to DB. This field is NOT inserted to DB.
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        ///True: ADD prices to extras and REDUCE prices from main products. For an e-food order. see: EffodTasks.MatchProductsExtras.  This field is NOT inserted to DB.
        /// </summary>
        public bool ReduceRecipeQnt { get; set; } = true;

        /// <summary>
        /// External delivery origin like Efood, ClickDelivery, Deliveras. This field is NOT inserted to DB.
        /// </summary>
        public string ExtDeliveryOrigin { get; set; } = "Unknown External Delivery System";
    }

    // ---------------------------------------------------------------------


    public class DA_OrderModelExt : DA_OrderModel
    {
        public string StoreDescr { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Mobile { get; set; }
        public string JobName { get; set; }
        public string PhoneComp { get; set; }
        public string Proffesion { get; set; }
        public string VatNo { get; set; }
        public string Doy { get; set; }
        public string Notes { get; set; }
        public string LastOrderNotes { get; set; }
        public string SecretNotes { get; set; }
    }


    // ---------------------------------------------------------------------



    public class DA_OrderDetails
    {
        public long Id { get; set; }

        public long DAOrderId { get; set; }

        /// <summary>
        /// Product.Id
        /// </summary>
        [Range(0, int.MaxValue)]
        public long ProductId { get; set; }


        /// <summary>
        /// Product.Id
        /// </summary>
        //[MaxLength(150)]
        //public string ProductCode { get; set; }

        /// <summary>
        /// περιγραφή προϊόντος
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Nullable<long> PriceListId { get; set; }

        /// <summary>
        /// Συνολικό ποσό (πριν την έκπτωση)
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Ποσότητα
        /// </summary>
        [Required]
        [Range(0.001, Double.MaxValue)]
        public decimal Qnt { get; set; }

        /// <summary>
        /// Ποσό έκπτωσης
        /// </summary>
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Συνολικό ποσό (μετά την έκπτωση)
        /// </summary>
        [Required]
        public decimal Total { get; set; }

        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Required]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// % ΦΠΑ
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal RateVat { get; set; }

        /// <summary>
        /// % Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal RateTax { get; set; }

        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Price-Discount-TotalVat-TotalTax
        /// </summary>
        [Required]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Remarks for main product of a order detail. Sended to OrderDetailInvoices Table on Store and on Field ItemRemark
        /// </summary>
        public string ItemRemark { get; set; }

        /// <summary>
        /// ProductCode. Not inserted to DB
        /// </summary>
        [StringLength(150)]
        public string ProductCode { get; set; }

        public List<DA_OrderDetailsExtrasModel> Extras { get; set; }

        /// <summary>
        /// Code for PriceList. Only for Omnirest PlugIn
        /// </summary>
        public string PriceListCode { get; set; }

        /// <summary>
        /// Code for selected vat. Only for Omnirest PlugIn
        /// </summary>
        public Nullable<int> VatCode { get; set; }

        /// <summary>
        /// Code for product category. Only for Omnirest PlugIn
        /// </summary>
        public string ProductCategoryCode { get; set; }

        /// <summary>
        /// Code for category. Only for Omnirest PlugIn
        /// </summary>
        public Nullable<int> CategoryId { get; set; }

        /// <summary>
        /// Preparation time for product. Only for Omnirest PlugIn
        /// </summary>
        public Nullable<DateTime> PreparationTime { get; set; }

        /// <summary>
        /// Type of special discount that is used. 0: Hit Loyalty, 1: Goodys discounts, 2: Vodafone discounts
        /// </summary>
        [Range(0, 2)]
        public Nullable<DA_OrderDetail_OtherDiscountEnum> OtherDiscount { get; set; }

        public override string ToString()
        {
            string orderDetailInfo = Id.ToString() + ": " + (Description ?? "") + " (" + (ProductCode ?? "") + ") " + Qnt.ToString() + "x " + Price.ToString() + " -" + Discount.ToString();
            return orderDetailInfo;
        }

    }

    //-----------------------------------------------------------
    /// <summary>
    /// class for holding cashed orders. see ExternalOrdersHelper
    /// </summary>
    //public class DA_OrderCashedModel: DA_OrderIdsModel
    //{
    //    /// <summary>
    //    /// time of last search into the cashed list
    //    /// </summary>
    //    public DateTime LastSearch { get; set; }
    //}

    //-----------------------------------------------------------
    public class DA_OrderDetailsExtrasModel
    {
        public long Id { get; set; }

        public long OrderDetailId { get; set; }

        [Range(0, int.MaxValue)]
        public long ExtrasId { get; set; } = 0;

        [StringLength(150)]
        public string ExtrasCode { get; set; }

        /// <summary>
        /// περιγραφή extras
        /// </summary>
        [Required]
        [MinLength(2)]
        public string Description { get; set; }

        /// <summary>
        /// Ποσότητα
        /// </summary>
        public decimal Qnt { get; set; }

        /// <summary>
        /// Συνολικό ποσό extras
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Required]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// % ΦΠΑ
        /// </summary>
        public decimal RateVat { get; set; }

        /// <summary>
        /// % Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal RateTax { get; set; }

        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total-TotalVat-TotalTax
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal NetAmount { get; set; }

        public bool ItemsChanged { get; set; }


        /// <summary>
        /// ExtraCode. Not inserted to DB
        /// </summary>
        public string ExtraCode { get; set; }


        /// <summary>
        /// Code for PriceList. Only for Omnirest PlugIn
        /// </summary>
        public string PriceListCode { get; set; }

        /// <summary>
        /// Code for selected vat. Only for Omnirest PlugIn
        /// </summary>
        public Nullable<int> VatCode { get; set; }

        /// <summary>
        /// Code for product category. Only for Omnirest PlugIn
        /// </summary>
        public string ProductCategoryCode { get; set; }

        /// <summary>
        /// Code for category. Only for Omnirest PlugIn
        /// </summary>
        public Nullable<int> CategoryId { get; set; }


        public override string ToString()
        {
            string orderDetailExtraInfo = Id.ToString() + ": " + (Description ?? "") + " (" + (ExtraCode ?? "") + ") " + Qnt.ToString() + "x " + Price.ToString();
            return orderDetailExtraInfo;
        }
    }


    //------------------------------------------------------------
    public class DA_OrderStatusModel
    {
        public Nullable<long> Id { get; set; }

        public long OrderDAId { get; set; }

        public Int16 Status { get; set; }

        public DateTime StatusDate { get; set; }

        /// <summary>
        /// If true then save as statustime store status time else server time
        /// </summary>
        public bool KeepStoreTimeStatus { get; set; } = false;
    }

    /// <summary>
    /// Extended model to update store after DA update
    /// </summary>
    public class DA_OrderStatusExtModel : DA_OrderStatusModel
    {
        public long StoreOrderStatusId { get; set; }
    }

    public class DA_ExtOrderModel
    {
        public DA_OrderModel OrderModel { get; set; }

        public List<DA_AddressModel> AddressesModel { get; set; }
    }

    /// <summary>
    /// Model encapsulating DA_OrderExtDeliveryModel, DACustomerModel and DA_AddressModel models (for External Delivery Systems and their Plugins)
    /// </summary>
    public class DA_ExtDeliveryModel
    {
        public DA_OrderExtDeliveryModel Order { get; set; }

        public DACustomerModel Customer { get; set; }

        public DA_AddressModel ShippingAddress { get; set; }
        public DA_AddressModel BillingAddress { get; set; }

        /// <summary>
        /// pricelist που θα χρησιμοποιηθεί
        /// </summary>
        public long PriceListId { get; set; }

        /// <summary>
        /// Default ProductId (To προϊόν πρέπει να περιέχει ένα ExtrasId). 
        /// Αν το code του προϊόντος που έρχεται από το external delivery system δε βρίσκεται στην DB, τότε το άγνωστο προϊόν αντιστοιχίζεται με το DefaultProductId
        /// </summary>
        public long DefaultProductId { get; set; }

        /// <summary>
        /// true: στη λίστα των extras δε συμπεριλαμβάνονται όσα ανήκουν στη Συνταγή (βλ. )
        /// </summary>
        public bool RemoveRecipeExtras { get; set; } = true;


    }
    // ---------------------------------------------------------------------

    public class DA_NewOrderModel : DA_OrderModel
    {
        public DASearchCustomerModel CustomerData { get; set; }

        public DA_StoreModel StoreModel { get; set; }

        public List<DA_OrderDetailExtModel> OrderDetails { get; set; }

        public int PosId { get; set; }

        public int PosStaffId { get; set; }

        public string Bell { get; set; }

    }

    public class UpdateRemarksModel
    {
        public long OrderId { get; set; }
        public string Remarks { get; set; }

        public string LastOrderNote { get; set; }
    }


    // ---------------------------------------------------------------------
    public class DA_SearchOrdersModel
    {
        public DateTime OrderDateFrom { get; set; }

        public DateTime OrderDateTo { get; set; }

        public long StoreId { get; set; }

        public long CustomerId { get; set; }

        public Int16 Status { get; set; }

        public Int16 OrderType { get; set; }

        public bool WithRemarks { get; set; }

        public int DurationLessThan { get; set; }

        public int DurationMoreThan { get; set; }
    }

    // ---------------------------------------------------------------------

    /// <summary>
    /// Model to send order from DA Server to Client  using Web Api Call
    /// </summary>
    public class OrderFromDAToClientForWebCallModel
    {
        public DA_OrderModel Order { get; set; }

        public List<DA_OrderDetails> OrderDetails { get; set; }

        public List<DA_OrderDetailsExtrasModel> OrderDetailExtras { get; set; }

        public DACustomerModel Customer { get; set; }

        public List<DA_AddressModel> Addresses { get; set; }


        public DA_StoreModel StoreModel { get; set; }


    }


    // ---------------------------------------------------------------------
    /// <summary>
    /// Faild Order To Send To Store From Server with Reason
    /// </summary>
    public class FailedDA_OrdersModels
    {
        public DA_OrderModel Order { get; set; }

        public string ErrorMess { get; set; }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// External Model for Da Orders
    /// </summary>
    public class ExternalObjectModel
    {
        public ExternalStaffModel Staff { get; set; }

        public List<ExternalCouponsModel> Coupons { get; set; }

        public LoyaltyCoupons LoyaltyCouponInfo { get; set; }
    }


    // ---------------------------------------------------------------------

    /// <summary>
    /// External Staff Model
    /// </summary>
    public class ExternalStaffModel
    {
        public Nullable<long> Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Identification { get; set; }

        public string Image { get; set; }
    }

    // ---------------------------------------------------------------------

    /// <summary>
    /// External Model for Coupons
    /// </summary>
    public class ExternalCouponsModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }
    }

    // ---------------------------------------------------------------------

    public class LoyaltyCoupons
    {
        public string CampaignName { get; set; }
        public string CouponCode { get; set; }
        public string CouponType { get; set; }
        public string LoyaltyId { get; set; }
        public List<LoyaltyCouponModel> Coupons { get; set; }
        public string ExternalOrderId { get; set; }
    }

    public class LoyaltyCouponModel
    {
        public string CampaignName { get; set; }
        public string CouponCode { get; set; }
        public string CouponType { get; set; }
    }

    // ---------------------------------------------------------------------

    /// <summary>
    /// External model for payment via bank system
    /// </summary>
    public class ExternalPaymentModel
    {
        [Required]
        public long OrderId { get; set; }
        [Required]
        public string PaymentId { get; set; }
    }

    /// <summary>
    /// Model to get orders to check status from Goodys Omnirest
    /// </summary>
    public class DA_OrdersForGoodysOmnirestStatus
    {
        /// <summary>
        /// da_Orders Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// DA_Orders Order No
        /// </summary>
        public long OrderNo { get; set; }

        /// <summary>
        /// Goodys Omnirest Order Id (DA_Orders ExtId1)
        /// </summary>
        public string OmnRestOrderId { get; set; }

        /// <summary>
        /// Different between OrderDate and GetDate in Hours.
        /// </summary>
        public int StayHours { get; set; }

        /// <summary>
        /// Current Status
        /// </summary>
        public int Status { get; set; }
    }
}
