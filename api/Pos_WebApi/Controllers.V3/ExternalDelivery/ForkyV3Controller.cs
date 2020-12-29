using log4net;
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Helpers;
using Pos_WebApi.Modules;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.ExternalDelivery
{
    [RoutePrefix("api/v3/Forkey")]
    public class ForkyV3Controller : BasicV3Controller
    {
        IForkeyFlows forkyflow;

        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ForkyV3Controller(IForkeyFlows _forkyflow)
        {
            this.forkyflow = _forkyflow;
        }

        /// <summary>
        /// Based on order Id collects order entities 
        /// Changes Extobj to isPrinted 
        /// Patches order to Printed 
        /// returns Invoice with invoice type 2 as Captains Order
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns> Invoice with invoice type 2 as Captains Order </returns>
        [HttpPost, Route("PrintCaptainsOrder/{orderid}/{extType}")]
        public HttpResponseMessage GetCaptainsOrder(long orderid, int? extType)
        {
            var res = forkyflow.PrintCaptainsOrderFlow(DBInfo, orderid, extType);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// 1 Get Lookups DeliveryCustomer + Basic PosInfo lookups
        /// 2 for Each order tryies to upsert customer
        /// 3 Creates Receipt for Invoice 
        /// 4 Uses invoices repo to create invoice
        /// 5 On success posts receipt back to patch route
        /// 6 On fail continues 
        /// </summary>
        /// <param name="forders"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertRange")]
        [ValidateModelState]
        public HttpResponseMessage InsertForkeyOrders(List<ForkeyDeliveryOrder> forders)
        {
            logger.Info("---- New Forky orders:" + forders.Count.ToString());
            ForkeyLookups lookups = new ForkeyLookups();
            try
            {
                lookups = forkyflow.GetForkeyOrderLookups(DBInfo);
                foreach (ForkeyDeliveryOrder forder in forders)
                {
                    logger.Info("-- Forky order Id:" + forder.id.ToString() + ", Status: " + forder.status);
                    try
                    {
                        if (forder.status == "PENDING")
                        {
                            //Insert Customer Create Receipt
                            Receipts nrec = forkyflow.InsertForkeyOrderFlow(DBInfo, forder, lookups);
                            //Use invoice repo to post invoice
                            InvoiceRepository repository = new InvoiceRepository(new PosEntities(false, DBInfo.Id));
                            dynamic recs = repository.UpdateInvoiceCounters(nrec);
                            logger.Info("    receiptNo :" + (recs.ReceiptNo ?? "<null>").ToString() + ", OrderNo: " + (recs.OrderNo ?? "<null>").ToString());
                            if (recs.OrderNo != null)
                            {
                                nrec.OrderNo = recs.OrderNo.ToString();
                                foreach (ReceiptDetails rd in nrec.ReceiptDetails) { rd.OrderNo = recs.OrderNo; }
                            }
                            //On success patch order
                            HttpResponseMessage resins = repository.MainPostReceiptsAction(DBInfo.Id.ToString(), new Receipts[] { nrec });
                            if (resins.StatusCode == HttpStatusCode.Created)
                            {
                                long r = forkyflow.PatchOrder(DBInfo, forder);
                            }
                        }
                        else if (forder.status == "CANCELLED")
                        {
                            ForkeyLocalEntities ents = forkyflow.GetForkeyOrderEntities(DBInfo, forder);
                            InvoiceModel inv = ents.InvoiceList.OrderByDescending(o => o.Day).FirstOrDefault();
                            long tocancelId = inv.Id ?? 0;
                            InvoiceRepository repository = new InvoiceRepository(new PosEntities(false, DBInfo.Id));
                            dynamic recs = repository.GetReceiptDetailsById(tocancelId);
                            if (recs.Receipt != null)
                            {
                                Receipts newRec = repository.ConvertTmpReceiptBOToReceiptForCancelled(recs.Receipt);
                                Receipts nrecc = forkyflow.CancelForkeyOrderFlow(newRec, forder, lookups);
                                //On success patch order
                                HttpResponseMessage resins = repository.MainPostReceiptsAction(DBInfo.Id.ToString(), new Receipts[] { nrecc });
                                //if (resins.StatusCode == HttpStatusCode.Created)
                                //{
                                long r = forkyflow.PatchOrder(DBInfo, forder);
                                //}
                            }
                        }
                        else
                        {
                            logger.Info("Forkey Order with Not Pending or Cancelled status:" + forder.status + " id:" + forder.id);
                            logger.Info(forder);
                        }
                    }
                    catch (ForkeyException fex)
                    {
                        if (fex.Message == DeliveryForkeyErrorEnum.ALLREADY_PROCESSED.ToString())
                        {
                            // manage fex on allready
                            try
                            {
                                long r = forkyflow.PatchOrder(DBInfo, forder);
                            }
                            catch (Exception ex) { logger.Error("Failed to patch order:" + ex.ToString()); }
                        }
                        else
                        {
                            // Handle here responce of order missmatch that depends on forkey functionality
                            forder.status = "error"; forder.error_reason = fex.ToString();
                            logger.Error(Environment.NewLine + "   >>>>>    ForkeyException on Insert :" + fex.ToString());
                            long r = forkyflow.PatchOrder(DBInfo, forder);

                        }
                        //Proccess order return signal
                        //Receipts nrec = forkyflow.InsertForkeyOrderFlow(Store, forder, lookups);
                        //Try to patch call 
                        continue;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString()); continue;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception ex)
            {
                logger.Error("Error while getting forkey lookups insert wont start ex:" + ex); throw ex;
            }
        }


        //{TODO: Old Code must remove}
        /// <summary>
        /// 1 Get Lookups DeliveryCustomer + Basic PosInfo lookups
        /// 2 for Each order tryies to upsert customer
        /// 3 Creates Receipt for Invoice 
        /// 4 Uses invoices repo to create invoice
        /// 5 On success posts receipt back to patch route
        /// 6 On fail continues 
        /// </summary>
        /// <param name="forders"></param>
        /// <returns></returns>
        //[HttpPost, Route("InsertDeliveryOrders")]
        //[ValidateModelState]
        //public HttpResponseMessage InsertDA_DeliveryOrders(OrderFromDAToClientForWebCallModel ServerOrder)
        //{

            
        //}


        [HttpPost, Route("PatchAndPrintForkeyOrder/{InvoiceId}")]
        public HttpResponseMessage PatchAndPrintForkeyOrder(Int64 InvoiceId)
        {
            bool rows = forkyflow.ChangeForkeyIsPrintedExtObj(DBInfo, InvoiceId, true);
            return Request.CreateResponse(HttpStatusCode.OK, rows);
        }


    }

}
