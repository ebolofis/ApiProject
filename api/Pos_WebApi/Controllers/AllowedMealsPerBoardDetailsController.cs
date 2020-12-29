using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class AllowedMealsPerBoardDetailsController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/AllowedMealsPerBoard
        public IEnumerable<AllowedMealsPerBoardDetails> GetAllowedMealsPerBoardDetails(string storeid)
        {
            return db.AllowedMealsPerBoardDetails.AsEnumerable();
        }

        // GET api/AllowedMealsPerBoard/5
        public IEnumerable<AllowedMealsPerBoardDetails> GetAllowedMealsPerBoardDetails(long AllowedMealsPerBoardId, string storeid, bool a = true)
        {
            return db.AllowedMealsPerBoardDetails.Where(w => w.AllowedMealsPerBoardId == AllowedMealsPerBoardId);
        }
       
        // GET api/AllowedMealsPerBoard/5
        public AllowedMealsPerBoardDetails GetAllowedMealsPerBoardDetails(long id, string storeid)
        {
            AllowedMealsPerBoardDetails model = db.AllowedMealsPerBoardDetails.Find(id);
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return model;
        }

        // PUT api/AllowedMealsPerBoard/5
        public HttpResponseMessage PutAllowedMealsPerBoardDetails(long id, string storeid, AllowedMealsPerBoardDetails model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != model.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(model).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("AllowedMealsPerBoardDetailsController PutAllowedMealsPerBoardDetails Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/AllowedMealsPerBoard
        public HttpResponseMessage PostAllowedMealsPerBoardDetails(AllowedMealsPerBoardDetails model, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.AllowedMealsPerBoardDetails.Add(model);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/AllowedMealsPerBoard/5
        public HttpResponseMessage DeleteAllowedMealsPerBoardDetails(long id, string storeid)
        {
            AllowedMealsPerBoardDetails model = db.AllowedMealsPerBoardDetails.Find(id);
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.AllowedMealsPerBoardDetails.Remove(model);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("AllowedMealsPerBoardDetailsController DeleteAllowedMealsPerBoardDetails Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
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
