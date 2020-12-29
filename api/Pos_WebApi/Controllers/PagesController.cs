using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class PagesController : ApiController
    {
     //   private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Pages
        public IEnumerable<Pages> GetPages()
        {
            try
            {
                using (PosEntities db = new PosEntities(false))
                {
                    var pages = db.Pages.Include(p => p.PageSet).AsNoTracking().ToList();
                    return pages.OrderBy(o => o.Sort).AsEnumerable();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

            
        }


        /// <summary>
        /// Get all active pages (pageSets and pageButtons are NOT included) for a specific POS for the current date ordered by Pages.Sort
        /// </summary>
        /// <param name="posid">posinfo.id</param>
        /// <returns>active pages for a specific POS for the current date ordered by Pages.Sort</returns>
        public IEnumerable<Pages> GetPagesForPosId(long posid)
        {
            try
            {
                DateTime dt = DateTime.Now;
                using (PosEntities db = new PosEntities(false))
                {
                    var query = db.PagePosAssoc.Include(i => i.PageSet)
                        .Where(w => w.PageSet.ActivationDate <= dt && w.PageSet.DeactivationDate >= dt && w.PosInfoId == posid).AsNoTracking()
                        .Select(s => s.PageSet.Pages).FirstOrDefault();
                    if (query == null)
                    {
                        throw new HttpException("Tip: Check for Activation Dates of Pageset");
                    }

                    return query.OrderBy(o => o.Sort).Where(o2 => o2.Status == true).AsEnumerable();

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }


        public IEnumerable<Pages> GetPages(int pagesetid)
        {
            try
            {
                using (PosEntities db = new PosEntities(false))
                {
                    var pages = db.Pages.Include(p => p.PageSet).Where(w => w.PageSetId == pagesetid);
                    return pages.OrderBy(o => o.Sort).AsEnumerable();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

        }

        // GET api/Pages/5
        public Pages GetPages(long id)
        {
            using (PosEntities db = new PosEntities(false))
            {
                Pages pages = db.Pages.Find(id);
                if (pages == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return pages;

            }
           
        }

        // PUT api/Pages/5
        public HttpResponseMessage PutPages(long id, Pages pages)
        {
            using (PosEntities db = new PosEntities(false))
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (id != pages.Id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                db.Entry(pages).State = EntityState.Modified;

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
           
        }

        // POST api/Pages
        public HttpResponseMessage PostPages(Pages pages)
        {

            using (PosEntities db = new PosEntities(false))
            {
                if (ModelState.IsValid)
                {
                    db.Pages.Add(pages);
                    db.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pages);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pages.Id }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

            }
           
        }

        // DELETE api/Pages/5
        public HttpResponseMessage DeletePages(long id)
        {

            using (PosEntities db = new PosEntities(false))
            {
                Pages pages = db.Pages.Find(id);
                if (pages == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                db.Pages.Remove(pages);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, pages);

            }
          
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
           
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
          //  db.Dispose();
            base.Dispose(disposing);
        }
    }
}