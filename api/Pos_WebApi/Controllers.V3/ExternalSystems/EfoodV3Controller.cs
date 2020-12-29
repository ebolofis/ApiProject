using Newtonsoft.Json;
using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.ExternalSystems
{
    [RoutePrefix("api/v3/da/Efood")]
    public class EfoodV3Controller : BasicV3Controller
    {
        IExtDeliverySystemsFlows efoodFlows;

        public EfoodV3Controller(IExtDeliverySystemsFlows efoodFlows)
        {
            this.efoodFlows = efoodFlows;
        }

        /// <summary>
        /// Get orders from efood web api. 
        /// ToDo : comment it IN  <<------<<<<<
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpGet, Route("getorders")]
        public HttpResponseMessage GetOrders()
        {
            efoodFlows.GetEFoodOrders();
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        /// <summary>
        /// Get orders from efood Bucket
        /// </summary>
        /// <returns>List<OrderEfood></returns>
        [HttpGet, Route("getBucket")]
        public HttpResponseMessage getBucket()
        {
            List<DA_ExtDeliveryModel> res= efoodFlows.getBucket(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK,res);
        }


        /// <summary>
        /// check e-food order again and insert order to DA_Orders or EfoodBucket 
        /// This action is used when OrderEfood had been MANUALLY repaired .
        /// Return the same OrderEfood with updated error messages. 
        ///   If error='' then order is inserted into DA_Orders and deleted from EfoodBucket.
        /// </summary>
        /// <param name="order">OrderEfood</param>
        [HttpPost, Route("Repost")]
        [Authorize]
       // [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage Repost(DA_ExtDeliveryModel order)
        {
            logger.Info("E-food, Reposting order " + order.Order.ExtId1 + "...");
            DA_ExtDeliveryModel res = efoodFlows.ResendDAOrder(DBInfo, order);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Mark As Deleted an item from Bucket
        /// </summary>
        /// <returns>List<OrderEfood></returns>
        [HttpGet, Route("markDeleteBucketItem/{id}")]
        public HttpResponseMessage MarkDeletedBucketItem(string id)
        {
            efoodFlows.MarkDeleted(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// E-food posts new orders
        /// </summary>
        /// <returns>List<OrderEfood></returns>
        [HttpPost, Route("post")]
        [AllowAnonymous]
        public HttpResponseMessage PostNewOrder(object model)
        {
            try
            {
                string ModelToJson = JsonConvert.SerializeObject(model);
                logger.Info("NEW EFOOD ORDER: " + ModelToJson);
            }
            catch (Exception ex)
            {
                logger.Error("NEW EFOOD ORDER Json ERROR" + ex.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
