
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalDelivery;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Flows.ExternalDelivery
{
    public class ForkeyFlows : IForkeyFlows
    {
        IForkeyTasks forktask;
        IDeliveryCustomerTasks dcTasks;
        
        public ForkeyFlows(IForkeyTasks _forktask, IDeliveryCustomerTasks _dcTasks)
        {
            this.forktask = _forktask;
            this.dcTasks = _dcTasks;
        }

        /// <summary>
        /// FLow to return Basic lookpups used to transform ForkeyOrders to Receipt objects 
        /// Contains list of [posinfo , posinfodetail, pricelists , salestypes , accounts , staff]
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public ForkeyLookups GetForkeyOrderLookups(DBInfoModel Store) => forktask.GetLookups(Store);

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
        public Receipts InsertForkeyOrderFlow(DBInfoModel Store, ForkeyDeliveryOrder forder, ForkeyLookups lookups)
        {
            DeliveryForkeyErrorEnum ErrorNum = forktask.CheckValidOrder(Store, forder, lookups);
            if (ErrorNum != DeliveryForkeyErrorEnum.NoError)
            {
                throw new ForkeyException(ErrorNum.ToString());
            }
            else
            {
                //upsert customer 
                DeliveryCustomerModel dcust = forktask.CreateOnlineCustomer(Store, forder, lookups.DeliveryCustomerLookups);
                if (dcust.ID == 0) { dcust = dcTasks.InsertCustomerTask(Store, dcust); }
                else { dcust = dcTasks.UpdateCustomerTask(Store, dcust); }
                //create order
                return forktask.ManageSingleForkeyOrder(Store, forder, dcust, lookups);
            }
        }

        //DeliveryCustomerModel UpsertCustomer(Store Store, Forkeydeli)

        /// <summary>
        /// On Cancel Forkey Order task calls and gets order by external Id 
        /// calls cancel order locally from lower layers and receipt DT and on success 
        /// Calls patch to cancel order
        /// </summary>
        /// <param name="Store">Store posted on new Order</param>
        /// <param name="model">Forkey Order from Delivery  Service etc etc</param>
        /// <returns>Id of order provided</returns>
        public Receipts CancelForkeyOrderFlow(Receipts rec, ForkeyDeliveryOrder forder, ForkeyLookups lookups) => forktask.ApplyCancelToReceipt(rec, forder, lookups);

        /// <summary>
        /// Based on Forkey Order asks and returns [ Order, Invoices, Statuses ] of current order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        public ForkeyLocalEntities GetForkeyOrderEntities(DBInfoModel Store, ForkeyDeliveryOrder forder) => forktask.GetForkeyOrderLocalEntitiesTask(Store, forder);

        /// <summary>
        /// Flow that defines forkey state based on order statuses and invoices and triggers patch order to post on forkey api state of responce
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        public long PatchOrder(DBInfoModel Store, ForkeyDeliveryOrder forder)
        {
            DeliveryForkeyStatusEnum forkeyState = forktask.DefineForkeyState(Store, forder);
            return forktask.PatchOrder(forder.id.ToString(), forkeyState);
        }

        /// <summary>
        /// When an orderstatus changed from delivery mask this function called from OrderStatusController to patch order to delivery service 
        /// </summary>
        /// <param name="storeid"> Used to grab store to patch order </param>
        /// <param name="orderid"> Id of order that changed its status </param>
        /// <param name="orderstatus"> Model of delivery status posted to DB </param>
        /// <returns></returns>
        public long PatchOrderFromStatus(string storeid, long orderid, int? extType) => forktask.PatchOrderFromStatus(storeid, orderid, extType);


        /// <summary>
        /// Provide storeid and order no to patch order if found to delivery forkey service
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public long CancelPatchOrderFromStatus(string storeid, string orderno) => forktask.CancelPatchOrderFromStatus(storeid, orderno);

        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        public bool ChangeForkeyIsPrintedExtObj(DBInfoModel Store, long InvoiceId, bool printed = true) => forktask.ChangeForkeyIsPrintedExtObj(Store, InvoiceId, printed);


        /// <summary>
        /// Based on order_id provided task gathers Forkey Entities from DTD and returns from invoicelist first invoice that 
        /// has invoicetype = 2 applyies extobj is printed = true and patches order to printed
        /// then returns invoice gothered to signal extcer
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public InvoiceModel PrintCaptainsOrderFlow(DBInfoModel store, long orderid, int? extType) => forktask.PrintCaptainsOrderTask(store, orderid, extType);
        //public List<DeliveryStatusOrders> CheckRendezvousDeliveryOrders(Store Store, DateTime? ClientTime) => forktask.CheckRendezvousDeliveryOrders(Store, ClientTime);

    }
}
