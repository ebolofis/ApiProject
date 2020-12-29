using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class OrderModel
    {
        public Nullable<long> Id { get; set; }
        public long OrderNo { get; set; }
        public DateTime Day { get; set; }
        public decimal Total { get; set; }
        public long PosId { get; set; }
        public long StaffId { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public long ReceiptNo { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public Nullable<int> PdaModuleId { get; set; }
        public Nullable<long> ClientPosId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        //null: apo tin efamorgi tou POS 
        //1: apo service tou hitposToWebpos bridge gia paragelies pou pane kateuthian kouzina 
        //2: apo service tou hitposToWebpos bridge gia paragelies pou paramenoun stis nees
        //3: apo service tou Forkey bridge
        public Nullable<int> ExtType { get; set; }

        //voithiko pedio pou sxetizete me ejoteriki paragelia
        public string ExtObj { get; set; }
        public string ExtKey { get; set; }

        //Proeleusi paragelias Local: 0, Web: 1, CallCenter: 2, MobileApp: 3.
        public int OrderOrigin { get; set; }
        public int Couver { get; set; }

        /// <summary>
        /// true: αλλαγή από DA
        /// </summary>
        public Nullable<bool> isDAModified { get; set; }

        /// <summary>
        /// Paid From DA
        /// </summary>
        public Nullable<bool> DA_IsPaid { get; set; }

        public Nullable<DateTime> EstTakeoutDate { get; set; }

        public Nullable<bool> IsDelay { get; set; }

        /// <summary>
        /// DA Notes For Order
        /// </summary>
        public string OrderNotes { get; set; }

        /// <summary>
        /// DA Notes For Store
        /// </summary>
        public string StoreNotes { get; set; }

        /// <summary>
        /// DA Notes For Customer
        /// </summary>
        public string CustomerNotes { get; set; }

        /// <summary>
        /// DA Secret Notes For Customer
        /// </summary>
        public string CustomerSecretNotes { get; set; }

        /// <summary>
        /// DA Last Order Notes For Customer
        /// </summary>
        public string CustomerLastOrderNotes { get; set; }

        /// <summary>
        /// Logical Error message etc. prices not same, different vats ....
        /// </summary>
        public string LogicErrors { get; set; }

        /// <summary>
        /// true: at leat one item changed
        /// </summary>
        public Nullable<bool> ItemsChanged { get; set; }

        /// <summary>
        /// προέλευση παραγγ.: 0: agent, 1: website, 2: mobile, 3: e-food
        /// </summary>
        public Nullable<Int16> DA_Origin { get; set; }


        public string LoyaltyCode { get; set; }
        public string TelephoneNumber { get; set; }

        public Nullable<int> CouverAdults { get; set; }

        public Nullable<int> CouverChildren { get; set; }

        public Nullable<long> DeliveryRoutingId { get; set; }
    }

    public class OrderFilterModel
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> PosId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public string OrderNo { get; set; }
        public string ExtKey { get; set; }
        public Nullable<int> ExtType { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

    }

    /// <summary>
    /// Result's after Delivery Agent Send an order to client
    /// </summary>
    public class ResultsAfterDA_OrderActionsModel
    {
        /// <summary>
        /// Delivery Agent Order Id
        /// </summary>
        public long DA_Order_Id { get; set; }

        /// <summary>
        /// Order Id From Client Store (Table Id)
        /// </summary>
        public long Store_Order_Id { get; set; }

        /// <summary>
        /// Order No From Client Store
        /// </summary>
        public long Store_Order_No { get; set; }

        /// <summary>
        /// Client Store Order Status
        /// </summary>
        public OrderStatusEnum Store_Order_Status { get; set; }

        /// <summary>
        /// Client Store Order Status Date
        /// </summary>
        public DateTime Store_Order_Status_DT { get; set; }

        /// <summary>
        /// If Succesfully posted to Client Store
        /// </summary>
        public bool Succeded { get; set; }

        /// <summary>
        /// Reason not posted
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        /// Client Store ExtEcr name to send order to kitchen if nedded
        /// </summary>
        public string ExtEcrName { get; set; }

        /// <summary>
        /// Client Store Invoice Id for Order
        /// </summary>
        public long InvoiceId { get; set; }

        /// <summary>
        /// PrintType for sending order to kitchen if nedded
        /// </summary>
        public PrintTypeEnum PrintType { get; set; }

        /// <summary>
        /// ItemAdditionalInfo for sending order to kitchen if nedded
        /// </summary>
        public string ItemAdditionalInfo { get; set; }

        /// <summary>
        /// TempPrint for sending order to kitchen if nedded
        /// </summary>
        public bool TempPrint { get; set; }

        /// <summary>
        /// Client Store Old Order Status
        /// </summary>
        public int Old_Store_Order_Status { get; set; }
    }

    /// <summary>
    /// Order Model with Association to all tables needed to be post
    /// </summary>
    public class FullOrderWithTablesModel : OrderModel
    {
        /// <summary>
        /// Order Detail Model with association for Order Deetail Ingrendients
        /// </summary>
        public List<OrderDetailWithExtrasModel> OrderDetails { get; set; }

        /// <summary>
        /// Invoice Model with association for Order Detail Invoices and Transaction
        /// </summary>
        public List<InvoiceWithTablesModel> Invoice { get; set; }

        /// <summary>
        /// Order Status Model
        /// </summary>
        public OrderStatusModel OrderStatus { get; set; }
        
    }

    /// <summary>
    /// Model for invoice with associated tables OrderDetailInvoices and Transaction
    /// </summary>
    public class InvoiceWithTablesModel : InvoiceModel
    {
        /// <summary>
        /// Transaction Model with associated table Invoice_Guest_Trans, CreditTransactions and TransferToPms
        /// </summary>
        public List<TransactionsExtraModel> Transactions { get; set; }

        /// <summary>
        /// List Of InvoiceShippingModel
        /// </summary>
        public List<InvoiceShippingDetailsModel> InvoiceShippings { get; set; }
    }


    /// <summary>
    /// External Object For extObj field based on DA_Orders table
    /// </summary>
    public class ExternalDA_ObjectModel : ExtForkeyObj
    {
        public ExternalObjectModel DA_ExternalModel { get; set; }

        public string AgentNo { get; set; }

        public Nullable<int> PointsGain { get; set; }

        public Nullable<int> PointsRedeem { get; set; }
    }

}
