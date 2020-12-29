using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Repositories.BORepos;
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
    public class RegionController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        RegionRepository repo;


        [Route("api/{storeId}/Region/GetAll")]
        public IEnumerable<RegionDTO> GetAll(string storeId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new RegionRepository(db);

            return repo.GetAll();
        }

        [Route("api/{storeId}/Region/GetPaged")]
        public PagedResult<RegionDTO> GetPaged(string storeId, string filters, int page, int pageSize)
        {
            //  var flts = JsonConvert.DeserializeObject<PageSetFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new RegionRepository(db);

            return repo.GetPaged(x => true, x1 => x1.Description, page, pageSize);
        }


        [Route("api/{storeId}/Region/Add")]
        [HttpPost]
        public HttpResponseMessage Insert(string storeId, RegionDTO model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new RegionRepository(db);

            repo.Add(model);
            try
            {
                if (repo.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        [Route("api/{storeId}/Region/UpdateRange")]
        [HttpPut]
        public HttpResponseMessage UpdateRange(string storeId, IEnumerable<RegionDTO> models)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new RegionRepository(db);

            if (ModelState.IsValid)
            {
                repo.UpdateRange(models);
                try
                {
                    if (repo.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, models);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/{storeId}/Region/DeleteRange")]
        [HttpDelete]
        public HttpResponseMessage DeleteRange(string storeId, IEnumerable<long> ids)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new RegionRepository(db);

            try
            {
                var res = repo.DeleteRange(ids);
                if (repo.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #region old Controller
        // GET api/Region
        public IEnumerable<Region> GetRegions(string storeid)
        {
            var res = db.Region.Include(i => i.Table).AsNoTracking().AsEnumerable();
            return res;//db.Region.Include(i=>i.Table).AsEnumerable();
        }

        // GET api/Region
        public IEnumerable<Region> GetRegions(string storeid, long posInfoId)
        {
            var res = db.PosInfo_Region_Assoc.Include(f => f.Region).Include(fff => fff.Region.Table).Where(ff => ff.PosInfoId == posInfoId).Select(e => e.Region);
            return res;//db.Region.Include(i=>i.Table).AsEnumerable();
        }


        /// <summary>
        /// Get regions based on posinfo.Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="notables">not used</param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public IEnumerable<Region> GetRegions(string storeid, bool notables, long posInfoId)
        {
            var sss = db.PosInfo_Region_Assoc.Include(f => f.Region).Where(f => f.PosInfoId == posInfoId).Select(f => f.Region);
            return sss;//db.Region.AsEnumerable();
        }

        public IEnumerable<Region> GetRegions(string storeid, long posid, bool notables, long posInfoId)
        {
            var res = db.PosInfo_Region_Assoc.Include(f => f.Region).Where(ff => ff.PosInfoId == posInfoId).Select(e => e.Region);
            return res;//db.Region.Where(w=>w.PosInfoId != null ? w.PosInfoId == posid : true).AsEnumerable();
        }

        // GET api/Region/5
        public Region GetRegion(long id, string storeid)
        {
            Region region = db.Region.Include(i => i.PosInfo_Region_Assoc).Where(w => w.Id == id).FirstOrDefault();
            if (region == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return region;
        }

        // PUT api/Region/5
        public HttpResponseMessage PutRegion(long id, string storeid, Region region)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != region.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var assocs = db.PosInfo_Region_Assoc.Where(w => w.RegionId == id).ToList();
            foreach (var asoc in assocs)
            {
                db.PosInfo_Region_Assoc.Remove(asoc);
            }
            foreach (var asoc in region.PosInfo_Region_Assoc)
            {
                db.PosInfo_Region_Assoc.Add(asoc);
            }

            db.Entry(region).State = EntityState.Modified;

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

        // POST api/Region
        public HttpResponseMessage PostRegion(Region region, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.Region.Add(region);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, region);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = region.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Region/5
        public HttpResponseMessage DeleteRegion(long id, string storeid)
        {
            Region region = db.Region.Find(id);
            if (region == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Region.Remove(region);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, region);
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
    #endregion
}