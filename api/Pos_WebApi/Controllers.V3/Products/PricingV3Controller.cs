using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Pricing")]
    public class PricingV3Controller : BasicV3Controller
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();
        IPricelistDetailFlows pricelistDetailFlow;

        public PricingV3Controller(IPricelistDetailFlows pricelistDetailFlow)
        {
            this.pricelistDetailFlow = pricelistDetailFlow;
        }

        /// <summary>
        /// Updates pricelist detail of specific product and specific pricelist with new price
        /// </summary>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <param name="newPrice"> New price value </param>
        /// <returns> Updated object </returns>
        [HttpPost, Route("UpdatePrice/product/{productId}/pricelist/{pricelistId}/price")]
        public HttpResponseMessage UpdatePrice(long productId, long pricelistId, double newPrice)
        {
            PricelistDetailModel res = pricelistDetailFlow.UpadteNewPriceForSpecificProductAndPricelist(DBInfo, productId, pricelistId, newPrice);
            hub.Clients.Group(DBInfo.Id.ToString()).updateOpenItemPrice(DBInfo.Id, res);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


/// <summary>
/// Updates pricelist detail of specific extra detail and specific pricelist with new price
/// </summary>
/// <param name="IngredientId"> Id of product </param>
/// <param name="PricelistDetailId"> Id of pricelist </param>
/// <param name="Price"> New price value </param>
/// <returns> Updated object </returns>
[HttpPost, Route("UpdatePrice/ingredient/{ingredientId}/pricelist/{pricelistdetailId}/price")]
public HttpResponseMessage UpdateExtraPrice(long IngredientId, long PriceListDetailId, double newPrice)
{
    PricelistDetailModel res = pricelistDetailFlow.UpdateExtraPrice(DBInfo, IngredientId, PriceListDetailId, newPrice);
    hub.Clients.Group(DBInfo.Id.ToString()).updateOpenItemPrice(DBInfo.Id, res);
    return Request.CreateResponse(HttpStatusCode.OK, res);
}
    }
}
