using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class PagePosAssocController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PagePosAssoc
        public IEnumerable<PagePosAssoc> GetPagePosAssocs()
        {
            var pageposassoc = db.PagePosAssoc.Include(p => p.PageSet).Include(p => p.PosInfo);
            return pageposassoc.AsEnumerable();
        }

        // GET api/PagePosAssoc/5
        public PagePosAssoc GetPagePosAssoc(long id)
        {
            PagePosAssoc pageposassoc = db.PagePosAssoc.Find(id);
            if (pageposassoc == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pageposassoc;
        }

        // PUT api/PagePosAssoc/5
        public HttpResponseMessage PutPagePosAssoc(long id, PagePosAssoc pageposassoc)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pageposassoc.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pageposassoc).State = EntityState.Modified;

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

        // POST api/PagePosAssoc
        public HttpResponseMessage PostPagePosAssoc(PagePosAssoc pageposassoc)
        {
            if (ModelState.IsValid)
            {
                db.PagePosAssoc.Add(pageposassoc);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pageposassoc);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pageposassoc.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PagePosAssoc/5
        public HttpResponseMessage DeletePagePosAssoc(long id)
        {
            PagePosAssoc pageposassoc = db.PagePosAssoc.Find(id);
            if (pageposassoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PagePosAssoc.Remove(pageposassoc);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pageposassoc);
        }

        // DELETE api/PagePosAssoc by pageset id
        public HttpResponseMessage DeletePagePosAssoc(bool isRange, long pagesetid)
        {
            IEnumerable<PagePosAssoc> pageset = db.PagePosAssoc.Where(w => w.PageSetId == pagesetid);
            if (pageset == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            foreach (PagePosAssoc p in pageset)
            {
                db.PagePosAssoc.Remove(p);
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pageset);
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