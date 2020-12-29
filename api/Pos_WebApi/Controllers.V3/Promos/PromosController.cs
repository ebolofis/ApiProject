using Symposium.Models.Models.Promos;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Promos
{
    /// <summary>
    /// Marketing promotions
    /// </summary>
    public class PromosController :  BasicV3Controller
    {


        IPromosFlows promosFlows;

        public PromosController(IPromosFlows promosFlows)
        {
            this.promosFlows = promosFlows;

        }

        /// <summary>
        /// Get DA Config  (DA_BaseUrl, StaffUserName, StaffPassword)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/promos/vodafone11")]
        [Route("api/v3/promos/vodafone11")]
        [Authorize]
        public HttpResponseMessage GetVodafone11()
        {
            List<Vodafone11Model> res = promosFlows.GetVodafone11Promos(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("api/v3/da/promos/vodafone11headers")]
        [Route("api/v3/promos/vodafone11headers")]
        [Authorize]
        public HttpResponseMessage GetVodafone11Headers()
        {
            List<Vodafone11Model> res = promosFlows.GetVodafone11HeaderPromos(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        [HttpGet, Route("api/v3/da/promos/vodafone11details/{HeaderId}")]
        [Route("api/v3/promos/vodafone11details/{HeaderId}")]
        [Authorize]
        public HttpResponseMessage GetVodafone11Details(long HeaderId)
        {
            List<Vodafone11ProdCategoriesModel> res = promosFlows.GetVodafone11DetailsPromos(DBInfo, HeaderId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("api/v3/da/promos/vodafone11alldetails")]
        [Route("api/v3/promos/vodafone11alldetails")]
        [Authorize]
        public HttpResponseMessage GetAllVodafone11Details()
        {
            List<Vodafone11ProdCategoriesModel> res = promosFlows.GetVodafoneAll11DetailsPromos(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpGet, Route("api/v3/da/promos/OneOffVodafone11/Id/{Id}")]
        [Route("api/v3/promos/OneOffVodafone11/Id/{Id}")]
        [Authorize]
        public HttpResponseMessage GetOneOffVodafone11Promos(long Id)
        {
            Vodafone11Model res = promosFlows.GetOneOffVodafone11Promos(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("api/v3/da/promos/insertVodafone11")]
        [Route("api/v3/promos/insertVodafone11")]
        [Authorize]
        public HttpResponseMessage InsertVodafone11(Vodafone11Model model)
        {
            long res = promosFlows.InsertVodafone11Promos(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("api/v3/da/promos/InsertVodafone11Details")]
        [Route("api/v3/promos/InsertVodafone11Details")]
        [Authorize]
        public HttpResponseMessage InsertVodafone11Details (Vodafone11ProdCategoriesModel model)
        {
            long res = promosFlows.InsertVodafone11DetailsPromos(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);

        }

        //[HttpPost, Route("api/v3/da/promos/InsertVodafone11Details")]
        //[Route("api/v3/promos/InsertVodafone11Details")]
        //[Authorize]
        //public HttpResponseMessage InsertDetailToHeader(Vodafone11ProdCategoriesModel model,long id)
        //{
        //    long res = promosFlows.InsertDetailToHeader(DBInfo, model,id);
        //    return Request.CreateResponse(HttpStatusCode.OK, res);

        //}

        [HttpPost, Route("api/v3/da/promos/DeleteVodafone11/Id/{Id}")]
        [Route("api/v3/promos/DeleteVodafone11/Id/{Id}")]
        [Authorize]
        public HttpResponseMessage DeleteVodafone11(long Id)
        {
            long res = promosFlows.DeleteVodafone11Promos(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("api/v3/da/promos/DeleteVodafone11Details/Id/{Id}")]
        [Route("api/v3/promos/DeleteVodafone11Details/Id/{Id}")]
        [Authorize]
        public HttpResponseMessage DeleteVodafone11Detail(long id)
        {
            long res = promosFlows.DeleteVodafone11DetailPromos(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);

        }

        [HttpPost, Route("api/v3/da/promos/UpdateVodafone11Headers")]
        [Route("api/v3/promos/UpdateVodafone11Headers")]
        [Authorize]
        public HttpResponseMessage UpdateVodafone11Promos(Vodafone11Model model)
        {
            Vodafone11Model res = promosFlows.UpdateVodafone11Promos(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);

        }

        [HttpPost, Route("api/v3/da/promos/UpdateVodafone11Details")]
        [Route("api/v3/promos/UpdateVodafone11Details")]
        [Authorize]
        public HttpResponseMessage UpdateVodafone11DetailsPromos(Vodafone11ProdCategoriesModel model)
        {
            Vodafone11ProdCategoriesModel  res = promosFlows.UpdateVodafone11DetailsPromos(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);

        }
    }
}
