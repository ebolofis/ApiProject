
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalDelivery
{
    public interface IForkeyFlows
    {

        /// <summary>
        /// FLow to return Basic lookpups used to transform ForkeyOrders to Receipt objects 
        /// Contains list of  [posinfo , posinfodetail, pricelists , salestypes , accounts , staff]
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        ForkeyLookups GetForkeyOrderLookups(DBInfoModel Store);

        /// <summary>
        /// On Insert new Order flow triggers task that upserts Delivery Customer Entities 
        /// Uses Delivery Customer tasks to upsert Delivery Customer
        /// ***********Returns Receipts model to Controller to post invoice on same lvl 
        /// Create WebPOS order and returns id of order provided if everything completes successully
        /// Flow Patch runs to complete action over current order to forkey connection
        /// </summary>
        /// <param name="Store">Store posted on new Order</param>
        /// <param name="model">Forkey Order from Delivery  Service etc etc</param>
        /// <returns>Id of order provided</returns>
        Receipts InsertForkeyOrderFlow(DBInfoModel Store, ForkeyDeliveryOrder forder, ForkeyLookups lookups);

        /// <summary>
        /// On Cancel Forkey Order task calls and gets order by external Id 
        /// calls cancel order locally from lower layers and receipt DT and on success 
        /// Calls patch to cancel order
        /// </summary>
        /// <param name="Store">Store posted on new Order</param>
        /// <param name="model">Forkey Order from Delivery Service etc etc</param>
        /// <returns>Id of order provided</returns>
        Receipts CancelForkeyOrderFlow(Receipts rec, ForkeyDeliveryOrder forder, ForkeyLookups lookups);

        /// <summary>
        /// Based on Forkey Order asks and returns [ Order, Invoices, Statuses ] of current order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        ForkeyLocalEntities GetForkeyOrderEntities(DBInfoModel Store, ForkeyDeliveryOrder forder);

        /// <summary>
        /// When an orderstatus changed from delivery mask this function called from OrderStatusController to patch order to delivery service 
        /// </summary>
        /// <param name="storeid"> Used to grab store to patch order </param>
        /// <param name="orderid"> Id of order that changed its status </param>
        /// <param name="orderstatus"> Model of delivery status posted to DB </param>
        /// <returns></returns>
        long PatchOrderFromStatus(string storeid, long orderid, int? extType);
        
        /// <summary>
        /// Provide storeid and order no to patch order if found to delivery forkey service
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        long CancelPatchOrderFromStatus(string storeid, string orderno);


        /// <summary>
        /// Flow that defines forkey state based on order statuses and invoices and triggers 
        /// patch order to post on forkey api state of responce
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        long PatchOrder(DBInfoModel Store,  ForkeyDeliveryOrder forder);

        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        bool ChangeForkeyIsPrintedExtObj(DBInfoModel Store, long InvoiceId, bool printed = true);

        /// <summary>
        /// Based on order_id provided task gathers Forkey Entities from DTD and returns from invoicelist first invoice that 
        /// has invoicetype = 2 applyies extobj is printed = true and patches order to printed
        /// then returns invoice gothered to signal extcer
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        InvoiceModel PrintCaptainsOrderFlow(DBInfoModel store, long orderid, int? extType);
    }
}
