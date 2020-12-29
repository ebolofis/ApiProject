
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalDelivery
{
    public interface IForkeyTasks
    {
        /// <summary>
        /// Task to return Basic lookpups used to transform ForkeyOrders to Receipt objects 
        /// Contains list of  [posinfo , posinfodetail, pricelists , salestypes , accounts , staff]
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        ForkeyLookups GetLookups(DBInfoModel dbInfo);

        /// <summary>
        /// Checks online order and returns an Enum Error enumeration to define error cause on online order processed
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        DeliveryForkeyErrorEnum CheckValidOrder(DBInfoModel dbInfo, ForkeyDeliveryOrder order, ForkeyLookups lookups);

        /// <summary>
        /// Uses tasks Methods to create addresses and phones 
        /// uses updatecustomer method to overwrite customer loaded wtih extID
        /// making all than current created entities unselected and upserts creates entities to lists of customer
        /// returns Delivery Customer ready for Upsert Method of DCustomers DT
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DeliveryCustomerModel CreateOnlineCustomer(DBInfoModel dbInfo, ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups);

        /// <summary>
        /// Customer from forkey order Creates a dummy customer to compare with local customer loaded by ext id from delivery system
        /// </summary>
        /// <param name="order"> Provided forkey order </param>
        /// <returns> A Delivery Customer based on WebPos DB </returns>
        DeliveryCustomerModel CustomerFromForkeyOrder(ForkeyDeliveryOrder dbInfo, DeliveryCustomerLookupModel lookups);

        /// <summary>
        /// Create a customer phone from order with lookup default type on phonetype first or 0 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="lookups"></param>
        /// <returns></returns>
        DeliveryCustomersPhonesModel PhoneFromForkeyOrder(ForkeyDeliveryOrder dbInfo, DeliveryCustomerLookupModel lookups);

        /// <summary>
        /// From forkey address model on order create Delivery Customer Address model
        /// </summary>
        /// <param name="order"> Forkey Order </param>
        /// <returns> Delivery Customer Order </returns>
        DeliveryCustomersShippingAddressModel ShippingAddressFromForkeyOrder(ForkeyDeliveryOrder dbInfo, DeliveryCustomerLookupModel lookups);

        /// <summary>
        /// Function that creates a dummy order billing address if forkey order is invoice
        /// </summary>
        /// <param name="order"> ForkeyOrder Provided by Delivery System </param>
        /// <returns> Billing Address if forkey order is invoice else  </returns>
        DeliveryCustomersBillingAddressModel BillingAddressFromForkeyOrder(ForkeyDeliveryOrder dbInfo, DeliveryCustomerLookupModel lookups);

        /// <summary>
        /// Provide a local loaded entity and an online model created from the order
        /// Manages Billing info if they are exist (exists means that order will be treated as an invoice **Timologio for forkey orders )
        /// Manages Addresses making them selected False and if address is found then it is been updated with values provided
        /// Also Address on Billing is only matched through ExtObj cause on forkey order it is not provided with id to match and only when invoicetype is Invoice
        /// </summary>
        /// <param name="localEntity"></param>
        /// <param name="online"></param>
        /// <returns></returns>
        DeliveryCustomerModel UpdateLocalCustomerFromOnline(DeliveryCustomerModel local, DeliveryCustomerModel online);

        /// <summary>
        /// Provided a forkey order a Local Customer Updated and Pos Entities as lookups 
        /// Constructs an External Object to define usage of order, Creates Receipt, Receipt Details from forkeyDT 
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <param name="localUser"></param>
        /// <param name="lookups"></param>
        /// <returns> Receipt as model to post to invoiceRepository </returns>
        Receipts ManageSingleForkeyOrder(DBInfoModel dbInfo, ForkeyDeliveryOrder order, DeliveryCustomerModel localUser, ForkeyLookups lookups);

        /// <summary>
        /// Calls ForketEntities DT to get Order, InvoicesList and OrderStatus List to return Enum
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="forder"></param>
        /// <returns> On Each Case of match Returns DeliveryForkeyStatusEnum based on state returned </returns>
        DeliveryForkeyStatusEnum DefineForkeyState(DBInfoModel dbInfo, ForkeyDeliveryOrder forder);

        /// <summary>
        /// Calls Forkey DT providing a filter for order to get entities or Local Order
        /// Order, Invoices, OrderStatuses
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        ForkeyLocalEntities GetForkeyOrderLocalEntitiesTask(DBInfoModel dbInfo, ForkeyDeliveryOrder forder);

        /// <summary>
        /// Gets a Receipt and a forkey order to change Receipt entities for cancel procedure via InvoiceRepo
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="forder"></param>
        /// <param name="lookups"></param>
        /// <returns></returns>
        Receipts ApplyCancelToReceipt(Receipts rec, ForkeyDeliveryOrder forder, ForkeyLookups lookups);

        /// <summary>
        /// Uses webconfiguration attributes of forkeyPatch, forkeyAuth to create url callback
        /// then uses IWebApiClientHelper to patch an order back to url with orderId 
        /// Call must be authenticated so url and auth is custom on webconfiguration.AppSettings
        /// </summary>
        /// <param name="forkey_orderId"></param>
        /// <param name="forkey_status"></param>
        /// <returns></returns>
        long PatchOrder(string forkey_orderId, DeliveryForkeyStatusEnum forkey_status);

        /// <summary>
        /// Gets forkey entities from DT and based on Order - Invoices - Order Statuses
        /// switches based on logic and patches Order using method on task 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        long PatchOrderFromStatus(string storeid, long orderid, int? extType);

        /// <summary>
        /// Based on orderno Provided gets forkey entities 
        /// Decides what is the appropriate State and uses patch order to post change on forkey 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        long CancelPatchOrderFromStatus(string storeid, string orderno);

        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        bool ChangeForkeyIsPrintedExtObj(DBInfoModel dbInfo, long InvoiceId, bool printed = true);

        /// <summary>
        /// Based on order_id provided task gathers Forkey Entities from DTD and returns from invoicelist first invoice that 
        /// has invoicetype = 2 applyies extobj is printed = true and patches order to printed
        /// then returns invoice gothered to signal extcer
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        InvoiceModel PrintCaptainsOrderTask(DBInfoModel dbInfo, long orderid, int? extType);

        /// <summary>
        /// Task Method to create Receipt Payments for Receipt creating from forkey order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ents"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        ReceiptPayments CreateReceiptPaymentsFromForkey(ForkeyDeliveryOrder order, ForkeyPosEntities ents, GuestModel guest);
    }
}
