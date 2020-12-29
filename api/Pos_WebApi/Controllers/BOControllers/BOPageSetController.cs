using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers {
    [RoutePrefix("api/{storeId}/BOPageSets")]
    public class BOPageSetController : ApiController {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        PageSetRepository svc;
        private PosEntities db = new PosEntities(false);

        public BOPageSetController() {
            svc = new PageSetRepository(db);
        }

        [Route("GetAllPageSets")]
        public IEnumerable<PageSetDTO> GetPageSets(string storeId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetAllPageSet();
        }

        [Route("GetPagedPageSets")]
        public PagedResult<PageSetDTO> GetPagedPageSets(string storeId, string filters, int page, int pageSize) {
            var flts = JsonConvert.DeserializeObject<PageSetFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetPagedPageSet(flts.predicate, x1 => x1.Description, page, pageSize);
        }

        [Route("GetPages")]
        public PagedResult<PagesDTO> GetPages(string storeId, string filters, int page, int pageSize) {
            var flts = JsonConvert.DeserializeObject<PagesFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetPagedPages(flts.predicate, x1 => x1.Description, page, pageSize);
        }

        [Route("GetAllPages/{pageSetId}")]
        public IEnumerable<PagesDTO> GetPageSets(string storeId, long pageSetId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetAllPages(x => x.PageSetId == pageSetId);
        }

        [Route("GetProducts")]
        public IEnumerable<TempProductsWithCategoriesFlat> GetProducts(string storeId, string filters) {
            var flts = JsonConvert.DeserializeObject<ProductsWithCategoriesFlatFilters>(filters);

            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetProducts(flts.predicate);
        }

        [Route("GetPagedProducts")]
        public PagedResult<TempProductsWithCategoriesFlat> GetProducts(string storeId, string filters, int page, int pageSize) {
            var flts = JsonConvert.DeserializeObject<ProductsWithCategoriesFlatFilters>(filters);

            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetPagedProducts(flts.predicate, x => x.ProductCode, page, pageSize);
        }

        [Route("GetButtons/{pageId}")]
        public IEnumerable<PageButtonDTO> GetButtons(string storeId, long pageId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            return svc.GetPageButtons(x => x.PageId == pageId);
        }

        [Route("AddPage")]
        [HttpPost]
        public HttpResponseMessage InsertPage(string storeId, PagesDTO model) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            if (ModelState.IsValid) {
                svc.AddPage(model);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                    hub.Clients.Group(storeId).refreshPageSet(model.PageSetId);

                    var addedModel = svc.GetAllPages(x => x.PageSetId == model.PageSetId && x.Description == model.Description).FirstOrDefault();
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, addedModel);
                    // response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                    return response;
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }


            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("UpdatePage")]
        [HttpPut]
        public HttpResponseMessage UpdatePage(string storeId, PagesDTO model) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            if (ModelState.IsValid) {
                svc.UpdateButtonsToPage(model);
                try {
                    if (svc.SaveChanges() == 0) {
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                    }
                    hub.Clients.Group(storeId).refreshPageSet(model.PageSetId);

                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
                var returnmodel = svc.GetAllPages(x => x.PageSetId == model.PageSetId && x.Description == model.Description).FirstOrDefault();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, returnmodel);
                //response.Headers.Location = new Uri(Url.Link("DefaultApi", modl);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


        [Route("AddPageSet")]
        [HttpPost]
        public HttpResponseMessage InsertPageSet(string storeId, PageSetDTO model) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            if (ModelState.IsValid) {
                svc.AddPageSet(model);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                    hub.Clients.Group(storeId).refreshPageSet(model.Id);
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                // response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("UpdatePageSet")]
        [HttpPut]
        public HttpResponseMessage UpdatePageSet(string storeId, PageSetDTO model) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            if (ModelState.IsValid) {
                svc.UpdatePageSet(model);

                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                    hub.Clients.Group(storeId).refreshPageSet(model.Id);

                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                //response.Headers.Location = new Uri(Url.Link("DefaultApi", modl);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [Route("DeletePageSet/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeletePageSet(string storeId, long id) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            try {
                var res = svc.DeletePageSet(id);
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
                hub.Clients.Group(storeId).refreshPageSet(-1);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("CopyFromPageSet")]
        [HttpPost]
        public HttpResponseMessage CopyFromPageSet(string storeId, PageSetDTO model, long sourcePageSetId) {

            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);

            try {
                var pageIds = model.Pages.Select(s => s.Id).ToList();
                model.Pages.Clear();
                var res = svc.CopyFromPageSet(model, sourcePageSetId, pageIds);
                hub.Clients.Group(storeId).refreshPageSet(res.Id);


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, res);
                // response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        /// <summary>
        /// UpdateFromPageSet resource is a function that handles current model and appends a copy of selected pages 
        /// Pages that are to copy is appended on model.Pages and has  model.Pages[i].PagesetId == sourcePageSetId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="model"></param>
        /// <param name="sourcePageSetId"></param>
        /// <returns></returns>
        [Route("UpdateFromPageSet")]
        [HttpPut]
        public HttpResponseMessage UpdateFromPageSet(string storeId, PageSetDTO model, long sourcePageSetId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new PageSetRepository(db);
            if (ModelState.IsValid) {
                try {
                    var pageIds = model.Pages.Where(cc => (cc.PageSetId != model.Id) && (cc.PageSetId == sourcePageSetId)).Select(s => s.Id).ToList();
                    model.Pages = model.Pages.Where(cc => cc.PageSetId == model.Id).ToList();

                    var res = svc.UpdateFromPageSet(model, sourcePageSetId, pageIds);
                    try {
                        if (svc.SaveChanges() == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                            hub.Clients.Group(storeId).refreshPageSet(res.Id);
                    } catch (DbUpdateConcurrencyException ex) {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, res);
                    return response;
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        protected override void Dispose(bool disposing) {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}
