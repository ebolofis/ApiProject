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
    public class MetaDataTableController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/MetadataTable
        public IEnumerable<MetadataTable> GetMetadataTable(string storeid)
        {
            return db.MetadataTable.AsEnumerable();
        }

        public IEnumerable<MetadataTable> GetMetadataTable(string storeid, int reportType)
        {
            return db.MetadataTable.Where(w => w.ReportType == reportType).AsEnumerable();
        }
        // GET api/MetadataTable/5
        public MetadataTable GetMetadataTable(long id, string storeid)
        {
            MetadataTable MetadataTable = db.MetadataTable.Find(id);
            if (MetadataTable == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return MetadataTable;
        }

        // PUT api/MetadataTable/5
        public HttpResponseMessage PutMetadataTable(long id, string storeid, MetadataTable model)
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

        // POST api/MetadataTable
        public HttpResponseMessage PostMetadataTable(MetadataTable model, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.MetadataTable.Add(model);
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

        // DELETE api/MetadataTable/5
        public HttpResponseMessage DeleteMetadataTable(long id, string storeid)
        {
            MetadataTable model = db.MetadataTable.Find(id);
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.MetadataTable.Remove(model);

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
