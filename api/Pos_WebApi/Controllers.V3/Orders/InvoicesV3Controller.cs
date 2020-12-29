using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{

    [RoutePrefix("api/v3/Invoices")]
    public class InvoicesV3Controller : BasicV3Controller
    {

        /// <summary>
        /// Main Logic Class
        /// </summary>
        IInvoicesFlows invoices;
        IGoodysFlow goodys;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public InvoicesV3Controller(IInvoicesFlows inv, IGoodysFlow gds)
        {
            this.invoices = inv;
            this.goodys = gds;
        }

        /// <summary>
        /// create aade qr code image, based on provided url and linked to provided invoiceid
        /// </summary>
        /// <param name="data">InvoiceQRPostModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("CreateInvoiceQR")]
        public HttpResponseMessage CreateInvoiceQR(InvoiceQRPostModel data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.url))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                long? id = invoices.CreateInvoiceQR(DBInfo, data.invoiceId, data.url);

                if (id != null && id > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "invoiceQR record was not created");
                }             
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message); 
            }
        }

        /// <summary>
        /// Cancels receipt based only on Invoice Id
        /// </summary>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns>
        /// <para> On success: 
        ///   OK on success  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpPost, Route("CancelReceipt/invoice/{InvoiceId}/pos/{PosInfoId}/staff/{StaffId}")]
        public HttpResponseMessage CancelReceipt(long InvoiceId, long PosInfoId, long StaffId)
        {

          

            try
            {
                SignalRAfterInvoiceModel dataForSignalR = invoices.CancelReceipt(DBInfo, InvoiceId, PosInfoId, StaffId, false);
                hub.Clients.Group(DBInfo.Id.ToString()).newReceipt(DBInfo.Id + "|" + dataForSignalR.ExtecrName, dataForSignalR.InvoiceId, dataForSignalR.useFiscalSignature, dataForSignalR.SendsVoidToKitchen, dataForSignalR.PrintType, dataForSignalR.ItemAdditionalInfo, dataForSignalR.TempPrint);

                //Send canceled receipt to hotelizer
                bool extApi = false;
                extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
                if (extApi)
                {
                    //post cancel receipt using Hotelizer Api Url
                    HotelizerFlows hotelizer = new HotelizerFlows();
                    hotelizer.CancelReceiptToHotelizer(InvoiceId, DBInfo);
                }


                string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
                if (customerClassType == "Pos_WebApi.Customer_Modules.Goodies")
                {
                    hub.Clients.Group(DBInfo.Id.ToString()).NewInvoice(PosInfoId, dataForSignalR.InvoiceId);
                    goodys.UpdateGoodysApi(InvoiceId, DBInfo);

                }
                if (dataForSignalR.TableId != null)
                {
                    hub.Clients.Group(DBInfo.Id.ToString()).refreshTable(DBInfo.Id, dataForSignalR.TableId);
                }
                hub.Clients.Group(DBInfo.Id.ToString()).kdsMessage(DBInfo.Id, dataForSignalR.kdsMessage);
                hub.Clients.Group(DBInfo.Id.ToString()).deliveryMessage(DBInfo.Id, dataForSignalR.deliveryMessage, dataForSignalR.SalesTypes);
                return Request.CreateResponse(HttpStatusCode.OK, dataForSignalR); // return results
            }
            catch(BusinessException bex)
            {
                logger.Warn("Message: " + bex.Message + ", Description: " + bex.Description);
                return Request.CreateResponse(HttpStatusCode.NotFound, bex.Message); //return 404
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }

            
        }

    }

}
