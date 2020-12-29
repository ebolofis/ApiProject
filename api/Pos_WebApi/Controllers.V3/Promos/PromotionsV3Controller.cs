using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Promos
{
    [RoutePrefix("api/v3/Promotions")]
    public class PromotionsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IPromotionsFlows PromotionsFlows;

        public PromotionsV3Controller(IPromotionsFlows promotionsFlows)
        {
            this.PromotionsFlows = promotionsFlows;
        }

        /// <summary>
        /// Get Promotions Model (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        [HttpGet, Route("Get")]
        public HttpResponseMessage GetAll()
        {
            List<PromotionsModels> res = PromotionsFlows.GetAll(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Promotions Model By HeaderId (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        [HttpGet, Route("Get/Ιd/{Id}")]
        public HttpResponseMessage GetPromotions(long Id)
        {
            PromotionsModels res = PromotionsFlows.GetPromotions(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Insert Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        [HttpPost, Route("Insert")]
        public HttpResponseMessage InsertPromotion(PromotionsModels Model)
        {
            long res = PromotionsFlows.InsertPromotion(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertCombo")]
        public HttpResponseMessage InsertCombo(PromotionsCombos Model)
        {
            long res = PromotionsFlows.InsertCombo(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertDiscount")]
        public HttpResponseMessage InsertDiscount(PromotionsDiscounts Model)
        {
            long res = PromotionsFlows.InsertDiscount(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        [HttpPost, Route("Update")]
        public HttpResponseMessage UpdatePromotion(PromotionsModels Model)
        {
            long res = PromotionsFlows.UpdatePromotion(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateCombo")]
        public HttpResponseMessage UpdateCombo(PromotionsCombos Model)
        {
            PromotionsCombos res = PromotionsFlows.UpdateCombo(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateDiscount")]
        public HttpResponseMessage UpdateDiscount(PromotionsDiscounts Model)
        {
            PromotionsDiscounts res = PromotionsFlows.UpdateDiscount(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        [HttpPost, Route("Delete/Ιd/{Id}")]
        public HttpResponseMessage DeletePromotion(long Id)
        {
            long res = PromotionsFlows.DeletePromotion(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("DeleteCombo/Ιd/{Id}")]
        public HttpResponseMessage DeleteCombo(long Id)
        {
            long res = PromotionsFlows.DeleteCombo(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("DeleteDiscount/Ιd/{Id}")]
        public HttpResponseMessage DeleteDiscount(long Id)
        {
            long res = PromotionsFlows.DeleteDiscount(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("UpsertPromotionHeader")]
        public HttpResponseMessage UpsertPromotionHeader(List<PromotionsHeaderSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = PromotionsFlows.InformTablesForPromotionHeaderFromDAServer(DBInfo, Model);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpsertPromotionCombos")]
        public HttpResponseMessage UpsertPromotionCombos(List<PromotionsCombosSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");
            UpsertListResultModel res = PromotionsFlows.InformTablesForPromotionCombosFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpsertPromotionDiscounts")]
        public HttpResponseMessage UpsertPromotionDiscounts(List<PromotionsDiscountsSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");
            UpsertListResultModel res = PromotionsFlows.InformTablesForPromotionDiscountFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        [HttpGet, Route("GetPromoPriceLists")]
        public HttpResponseMessage GetPromotionsPricelists()
        {
            List<PromotionsPricelistModel> res = PromotionsFlows.GetPromotionsPricelists(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        [HttpPost, Route("UpsertPromotionPricelists")]
        public HttpResponseMessage UpsertPromotionPricelists(List<PromotionsPricelistModel> Model)
        {
            List<PromotionsPricelistModel> res = PromotionsFlows.UpsertPromotionPricelists(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}