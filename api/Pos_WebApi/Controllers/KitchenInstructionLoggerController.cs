using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class KitchenInstructionLoggerController : ApiController
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        private PosEntities db = new PosEntities(false);
        private GenericRepository<KitchenInstructionLogger> svc;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public KitchenInstructionLoggerController()
        {
            svc = new GenericRepository<KitchenInstructionLogger>(db);
        }

        // GET api/KitchenInstructionLogger

        public IEnumerable<KitchenInstructionLogger> GetKitchenInstructionLoggers()
        {
            return svc.GetAll();
        }
        public PagedResult<KitchenInstructionLogger> GetKitchenInstructionLogger(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        // GET api/KitchenInstructionLogger/5
        public KitchenInstructionLogger GetKitchenInstructionLogger(long id)
        {

            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }
        // PUT api/KitchenInstructionLogger/5
        [HttpPut]
        public HttpResponseMessage PutKitchenInstructionLogger(IEnumerable<KitchenInstructionLogger> model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            svc.UpdateRange(model);

            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        public HttpResponseMessage PutKitchenInstructionLoggerStatus(string storeId, long kitchenInstructionLoggerId, short status)
        {

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            var updModel = svc.FindBy(x => x.Id == kitchenInstructionLoggerId).FirstOrDefault();
            updModel.Status = status;
            updModel.ReceivedTS = DateTime.Now;

            svc.Update(updModel);

            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/KitchenInstructionLogger
        public HttpResponseMessage PostKitchenInstructionLogger(string storeId, KitchenInstructionLogger model)
        {
            if (ModelState.IsValid)
            {
                model.SendTS = DateTime.Now;
                svc.Add(model);
                try
                {
                    var pi = db.PosInfo.Find(model.PosInfoId);
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                   // var updatedModel = svc.FindBy(x => x.PosInfoId == model.PosInfoId && x.SendTS == model.SendTS && x.KicthcenInstuctionId == model.KicthcenInstuctionId && x.StaffId == model.KicthcenInstuctionId).FirstOrDefault();
                    var kiJson = svc.GetJsonForFiscal(model);
                    hub.Clients.Group(storeId).KitchenInstructionLogger(storeId + "|" + model.ExtecrName, pi.Description, model.Id, kiJson);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                   // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error PostKitchenInstructionLogger :" + model.Description + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/KitchenInstructionLogger/5
        public HttpResponseMessage DeleteKitchenInstructionLogger(long id)
        {
            svc.Delete(x => x.Id == id);
            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}


