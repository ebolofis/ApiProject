using Pos_WebApi.Modules;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    //[RoutePrefix("api/v3/da/Loyalty")]
    public class DA_LoyaltyController : BasicV3Controller
    {
        IDA_LoyaltyFlows loyaltyFlow;

        public DA_LoyaltyController(IDA_LoyaltyFlows _loyaltyFlow)
        {
            this.loyaltyFlow = _loyaltyFlow;
        }


        /// <summary>
        /// Get Da_LoyalPoints List
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Loyalty/GetCustomerDa_LoyalPointsHistory/{CustomerId}")]
        [Route("api/v3/Loyalty/GetCustomerDa_LoyalPointsHistory/{CustomerId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCustomerDa_LoyalPointsHistory(long CustomerId)
        {
            List<DA_LoyalPointsModels> res = loyaltyFlow.GetCustomerDa_LoyalPointsHistory(DBInfo,CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Loyalty Configuration Tables
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_Loyalty  εκτός του DA_LoyalPoints</returns>
        [HttpGet, Route("api/v3/da/Loyalty/GetLoyaltyConfig")]
        [Route("api/v3/Loyalty/GetLoyaltyConfig")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetLoyaltyConfig()
        {
            DA_LoyaltyFullConfigModel res = loyaltyFlow.GetLoyaltyConfig(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Set Loyalty Configuration Tables
        /// </summary>
        /// <param name="Model">DA_LoyaltyFullConfigModel</param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/SetLoyaltyConfig")]
        [Route("api/v3/Loyalty/SetLoyaltyConfig")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage SetLoyaltyConfig(DA_LoyaltyFullConfigModel Model)
        {
            long res = loyaltyFlow.SetLoyaltyConfig(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Insert Loyalty Gain Amount Range Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/InsertGainAmountRange")]
        [Route("api/v3/Loyalty/InsertGainAmountRange")]
        [Authorize]
        public HttpResponseMessage InsertGainAmountRange(DA_LoyalGainAmountRangeModel Model)
        {
            long res = loyaltyFlow.InsertGainAmountRange(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Loyalty Gain Points Range Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/DeleteRangeRow/{Id}")]
        [Route("api/v3/Loyalty/DeleteRangeRow/{Id}")]
        [Authorize]
        public HttpResponseMessage DeleteRangeRow(long Id)
        {
            long res = loyaltyFlow.DeleteRangeRow(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delte All Loyalty Gain Amount Range
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/DeleteGainAmountRange")]
        [Route("api/v3/Loyalty/DeleteGainAmountRange")]
        [Authorize]
        public HttpResponseMessage DeleteGainAmountRange()
        {
            long res = loyaltyFlow.DeleteGainAmountRange(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Insert Redeem Free Product Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/InsertRedeemFreeProduct")]
        [Route("api/v3/Loyalty/InsertRedeemFreeProduct")]
        [Authorize]
        public HttpResponseMessage InsertRedeemFreeProduct(DA_LoyalRedeemFreeProductModel Model)
        {
            long res = loyaltyFlow.InsertRedeemFreeProduct(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Loyalty Redeem Free Product Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/DeleteRedeemFreeProductRow/{Id}")]
        [Route("api/v3/Loyalty/DeleteRedeemFreeProductRow/{Id}")]
        [Authorize]
        public HttpResponseMessage DeleteRedeemFreeProductRow(long Id)
        {
            long res = loyaltyFlow.DeleteRedeemFreeProductRow(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delte All Redeem Free Product
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/DeleteRedeemFreeProduct")]
        [Route("api/v3/Loyalty/DeleteRedeemFreeProduct")]
        [Authorize]
        public HttpResponseMessage DeleteRedeemFreeProduct()
        {
            long res = loyaltyFlow.DeleteRedeemFreeProduct(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Find Total Loyalty Point of a Customer
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns>Tο σύνολο των πόντων του πελάτη </returns>
        [HttpGet, Route("api/v3/da/Loyalty/GetLoyaltyPoints/Id/{Id}")]
        [Route("api/v3/Loyalty/GetLoyaltyPoints/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetLoyaltyPoints(long Id)
        {
            int res = loyaltyFlow.GetLoyaltyPoints(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Return the list of last Loyalty entries of the Current Customer
        /// </summary>
        /// <param name="entries">The Number of last entries to return. 0 for all entries</param>
        /// <param name="ptype">Type of entries to return: 0: points with no order (OrderId=0), 1: points with order (OrderId>0), 3: both </param>
        /// <returns>Return data from DA_LoyalPoints order by Date </returns>
        [HttpGet, Route("api/v3/da/Loyalty/Customer/GetLoyaltyPoints/entries/{entries}/type/{ptype}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCustomerLoyaltyPoints(int entries, DA_LoyaltyHistory ptype)
        {
            long custId = CustomerId;
            if (custId <= 0) throw new BusinessException("Invalid Customer");

            List<DA_LoyalPointsModels> res = loyaltyFlow.GetCustomerLoyaltyPointsHistory(DBInfo, custId, entries, ptype);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Choose Loyalty Redeem Options
        /// </summary>
        /// <returns>Επιστρέφει λίστα με επιλογές  που έχει ο πελάτης(κατά τη διάρκεια της παραγγελίας του) να καταναλώσει τους  πόντους του</returns>
        [HttpPost, Route("api/v3/da/Loyalty/RedeemOptions")]
        [Route("api/v3/Loyalty/RedeemOptions")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage GetLoyaltyRedeemOptions(DA_LoyaltyRedeemModel loyaltyRedeemModel)
        {
            DA_LoyaltyRedeemOptionsModel res = loyaltyFlow.GetLoyaltyRedeemOptions(DBInfo, loyaltyRedeemModel.Id, loyaltyRedeemModel.Amount);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// θέτει πόντους (gained/redeemed) από παραγγελία ΚΑΤΑΣΤΗΜΑΤΟΣ ΜΟΝΟ. Return gained points
        /// </summary>
        /// <param name="model">DA_LoyaltyStoreSetPointsModel</param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/SetPoints")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage setPointsFromStore(DA_LoyaltyStoreSetPointsModel model)
        {
            int gained = loyaltyFlow.setPointsFromStore(DBInfo, model, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, gained);
        }

        /// <summary>
        /// υπολογίζει τους κερδισμένους πόντους (χωρίς να τους εισάγει στην DB)  από παραγγελία ΚΑΤΑΣΤΗΜΑΤΟΣ ΜΟΝΟ. Return gained points
        /// </summary>
        /// <param name="model">DA_LoyaltyStoreSetPointsModel</param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/CalcPoints")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage calcPointsFromStore(DA_LoyaltyStoreSetPointsModel model)
        {
            int gained = loyaltyFlow.CalcPointsFromOrder(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, gained);
        }

        /// <summary>
        /// υπολογίζει τους κερδισμένους πόντους (χωρίς να τους εισάγει στην DB)  από παραγγελία DA MONO. Return gained points
        /// </summary>
        /// <param name="daOrder">DA_OrderModel</param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/CalcDAPoints")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage calcPointsFromStore(DA_OrderModel daOrder)
        {
            int gained = loyaltyFlow.CalcPointsFromOrder(DBInfo, daOrder);
            return Request.CreateResponse(HttpStatusCode.OK, gained);
        }


        /// <summary>
        /// αποθηκεύει τους κερδισμένους πόντους στο πίνακα  σε συνδυασμό με το Staff_id που πραγματοποίησε στο πίνακα DA_LoyalPoints?
        /// </summary>
        /// <param name="daOrder">DA_OrderModel</param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Loyalty/SaveDALoyaltyPointsByStaffId")]
        [Authorize]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage SavePointsFromOrder(DA_LoyalPointsModels model)
        {
            loyaltyFlow.SavePointsFromLoyaltyAdmin(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}