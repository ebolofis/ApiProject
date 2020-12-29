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
    public class MealConsumptionController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/MealConsumption
        public IEnumerable<MealConsumption> GetMealConsumption(string storeid)
        {
            return db.MealConsumption.AsEnumerable();
        }

        public IEnumerable<dynamic> GetMealConsuptionsForReservation(string storeid, Int64 resId)
        {
            var consumptions = from q in db.MealConsumption.Where(w => w.EndOfDayId == null && w.ReservationId == resId)
                               join qq in db.Guest on q.GuestId equals qq.Id
                               join qqq in db.PosInfo on q.PosInfoId equals qqq.Id
                               join qqqq in db.Department on q.DepartmentId equals qqqq.Id
                               join qqqqq in db.Staff on q.StaffId equals qqqqq.Id
                               select new
                               {
                                   Id = q.Id,
                                   ConsumedTS = q.ConsumedTS,
                                   BoardCode = q.BoardCode,
                                   Guest = qq.LastName,
                                   Department = qqqq.Description,
                                   PosInfo = qqq.Description,
                                   ConsumedMeals = q.ConsumedMeals,
                                   ConsumedChildrenMeals = q.ConsumedMealsChild,
                                   Staff = q.Staff
                               };
            return consumptions.OrderBy(o => o.ConsumedTS);
        }

        public IEnumerable<dynamic> GetHistoricMealConsuptionsForReservation(string storeid, Int64 resId, string type)
        {
            var consumptions = from q in db.MealConsumption.Where(w => w.ReservationId == resId)
                               join qq in db.Guest on q.GuestId equals qq.Id
                               join qqq in db.PosInfo on q.PosInfoId equals qqq.Id
                               join qqqq in db.Department on q.DepartmentId equals qqqq.Id
                               join qqqqq in db.Staff on q.StaffId equals qqqqq.Id
                               select new
                               {
                                   Id = q.Id,
                                   ConsumedTS = q.ConsumedTS,
                                   BoardCode = q.BoardCode,
                                   Guest = qq.LastName,
                                   Department = qqqq.Description,
                                   PosInfo = qqq.Description,
                                   ConsumedMeals = q.ConsumedMeals,
                                   ConsumedChildrenMeals = q.ConsumedMealsChild,
                                   Staff = q.Staff
                               };
            return consumptions.OrderBy(o => o.ConsumedTS);
        }

        // GET api/MealConsumption/5
        public MealConsumption GetMealConsumption(long id, string storeid)
        {
            MealConsumption model = db.MealConsumption.Find(id);
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return model;
        }

        // PUT api/MealConsumption/5
        public HttpResponseMessage PutMealConsumption(long id, string storeid, MealConsumption model)
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
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/MealConsumption
        public HttpResponseMessage PostMealConsumption(MealConsumption model, string storeid)
        {
            if (ModelState.IsValid)
            {
                model.ConsumedTS = DateTime.Now;
                db.MealConsumption.Add(model);
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

        // DELETE api/MealConsumption/5
        public HttpResponseMessage DeleteMealConsumption(long id, string storeid)
        {
            MealConsumption model = db.MealConsumption.Find(id);
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.MealConsumption.Remove(model);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
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
