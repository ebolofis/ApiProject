using log4net;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.ExternalSystems
{
    //[RoutePrefix("api/v3/Goodys")]
    public class GoodysV3Controller : BasicV3Controller
    {

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IGoodysFlow goodysFlows;
        IGoodysOrdersFlow goodysOrders;


        public GoodysV3Controller(IGoodysFlow _goodysFlows, IGoodysOrdersFlow _goodysOrders)
        {
            this.goodysFlows = _goodysFlows;
            goodysOrders = _goodysOrders;
        }

        [HttpPost, Route("api/v3/Goodys/GetCouponDetails")]
        public HttpResponseMessage GetCouponDetails(GoodysCouponDetailInfo orderInfo)
        {
            string couponDetails = goodysFlows.GetCouponDetails(DBInfo, orderInfo);
            return Request.CreateResponse(HttpStatusCode.OK, couponDetails);
        }
        [HttpGet, Route("api/v3/Goodys/GetOpenOrders")]
        public HttpResponseMessage GetOpenOrders()
        {
            long openOrders = goodysFlows.GetOpenOrders(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, openOrders);
        }

        [HttpPost, Route("api/v3/da/goodys/order/create")]
        [AllowAnonymous]
        public HttpResponseMessage PostOrder(GoodysDA_OrderModel model)
        {
            try
            {
                GoodysDA_OrderModel res = goodysOrders.InsertOrder(DBInfo, model);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                List<string> res = new List<string>();
                res.Add("AN ERROR HAS OCCURED ON ORDER CREATION");
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }






        [HttpGet, Route("api/v3/da/goodys/service/user/login/{AccountId}")]
        [AllowAnonymous]
        public HttpResponseMessage Login(string AccountId)
        {
            try
            {
                GoodysLoginResponceModel res = goodysOrders.GetLoginResponceModel(DBInfo, AccountId, false);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                List<string> res = new List<string>();
                res.Add("AN ERROR HAS OCCURED ON USER LOGIN");
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }

        [HttpPost, Route("api/v3/da/goodys/service/user/register")]
        [AllowAnonymous]
        public HttpResponseMessage RegisterUser(GoodysRegisterModel model)
        {
            try
            {
                GoodysLoginResponceModel res = goodysOrders.RegisterCustomer(DBInfo, model);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                List<string> res = new List<string>();
                res.Add("AN ERROR HAS OCCURED ON USER REGISTRATION");
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }

        [HttpPost, Route("api/v3/da/goodys/service/address/create")]
        [AllowAnonymous]
        public HttpResponseMessage CreateNewAddress(GoodysLoginAddressResponceModel model)
        {
            try
            {
                GoodysLoginResponceModel res = goodysOrders.CreateNewAddress(DBInfo, model);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                List<string> res = new List<string>();
                res.Add("AN ERROR HAS OCCURED ON ADDRESS CREATION");
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }

        [HttpPost, Route("api/v3/da/goodys/service/address/delete/{addressId}/{accountId}")]
        [AllowAnonymous]
        public HttpResponseMessage DeleteAddress(string addressId, string accountId)
        {
            try
            {
                GoodysLoginResponceModel res = goodysOrders.DeleteAddress(DBInfo, addressId, accountId);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                List<string> res = new List<string>();
                res.Add("AN ERROR HAS OCCURED ON DELETE ADDRESS");
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }
    }
}
