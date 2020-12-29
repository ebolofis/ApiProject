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
    public class PageSetController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PageSet
        public IEnumerable<PageSet> GetPageSets()
        {
          

            return db.PageSet.Include(i => i.PagePosAssoc).Include("PagePosAssoc.PosInfo").AsEnumerable();
        }

        // GET api/PageSet/5
        public PageSet GetPageSet(long id)
        {
            PageSet pageset = db.PageSet.Find(id);
            if (pageset == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pageset;
        }

        // PUT api/PageSet/5
        public HttpResponseMessage PutPageSet(long id, PageSet pageset)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pageset.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var delPagePosAssoc = db.PagePosAssoc.Where(w => w.PageSetId == pageset.Id);
            foreach (var d in delPagePosAssoc)
            {
                db.PagePosAssoc.Remove(d);
            }
            if (pageset.PagePosAssoc.Count > 0)
            {
                bool isOverlapping = false;
                var a = db.PageSet.Include(i => i.PagePosAssoc).Where(w => w.ActivationDate <= pageset.ActivationDate && w.DeactivationDate >= pageset.ActivationDate);
                var b = db.PageSet.Include(i => i.PagePosAssoc).Where(w => w.ActivationDate >= pageset.ActivationDate && w.ActivationDate <= pageset.DeactivationDate);
                var join = a.Union(b);
                foreach (var p in join)
                {
                    foreach (var newpos in pageset.PagePosAssoc)
                    {
                        if (p.PagePosAssoc.Select(s => s.PosInfoId).Contains(newpos.PosInfoId))
                        {
                            isOverlapping = true;
                            return Request.CreateErrorResponse(HttpStatusCode.Conflict, new Exception("Overlapping Period of PageSet which has same Modules in use!"));
                        }
                    }
                }
            }
            foreach (var p in pageset.PagePosAssoc)
            {
                p.PageSet = null;
                db.PagePosAssoc.Add(p);
            }
            var oldpageset = db.PageSet.Find(pageset.Id);
            if (oldpageset != null)
            {
                oldpageset.ActivationDate = pageset.ActivationDate;
                oldpageset.DeactivationDate = pageset.DeactivationDate;
                oldpageset.Description = pageset.Description;
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

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/PageSet
        public HttpResponseMessage PostPageSet(PageSet pageset)
        {
            if (ModelState.IsValid)
            {
                if (pageset.PagePosAssoc.Count > 0)
                {
                    bool isOverlapping = false;
                    var a = db.PageSet.Include(i => i.PagePosAssoc).Where(w => w.ActivationDate <= pageset.ActivationDate && w.DeactivationDate >= pageset.ActivationDate);
                    var b = db.PageSet.Include(i => i.PagePosAssoc).Where(w => w.ActivationDate >= pageset.ActivationDate && w.ActivationDate <= pageset.DeactivationDate);
                    var join = a.Union(b);
                    foreach (var p in join)
                    {
                        foreach (var newpos in pageset.PagePosAssoc)
                        {
                            if (p.PagePosAssoc.Select(s => s.PosInfoId).Contains(newpos.PosInfoId))
                            {
                                isOverlapping = true;
                                return Request.CreateErrorResponse(HttpStatusCode.Conflict, new Exception("Overlapping Period of new PageSet which has same Modules in use!"));
                            }
                        }
                    }
                }
                db.PageSet.Add(pageset);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pageset);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pageset.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // POST api/PageSet
        public HttpResponseMessage PostPageSet(PageSet pageset, int pageSetToCopyId)
        {
            if (ModelState.IsValid)
            {

                // clear page assoc connection
                pageset.PagePosAssoc.Clear();


                // add copied pages
                var pages = db.Pages.Include("PageButton").Include("PageButton.PageButtonDetail").Where(f => f.PageSetId == pageSetToCopyId);
                foreach (var page in pages)
                {
                    // add page
                    var pagetoadd = new Pages()
                    {
                        // PageSetId = pageset.Id,
                        Sort = page.Sort,
                        Status = page.Status,
                        ExtendedDesc = page.ExtendedDesc,
                        Description = page.Description,
                        DefaultPriceList = page.DefaultPriceList,
                    };

                    foreach (var pbutton in page.PageButton)
                    {
                        // add pagebutton
                        var pbuttontoadd = new PageButton()
                        {
                            Background = pbutton.Background,
                            Color = pbutton.Color,
                            Description = pbutton.Description,
                            ImageUri = pbutton.ImageUri,
                            KdsId = pbutton.KdsId,
                            KitchenCode = pbutton.KitchenCode,
                            KitchenId = pbutton.KitchenId,
                            NavigateToPage = pbutton.NavigateToPage,
                            PreparationTime = pbutton.PreparationTime,
                            PriceListDetailId = pbutton.PriceListDetailId,
                            PriceListId = pbutton.PriceListDetailId,
                            ProductId = pbutton.ProductId,
                            SetDefaultPriceListId = pbutton.SetDefaultPriceListId,
                            Sort = pbutton.Sort,
                            Type = pbutton.Type,
                            Vat = pbutton.Vat,
                            VatCode = pbutton.VatCode,
                            VatId = pbutton.VatId,
                            SetDefaultSalesType = pbutton.SetDefaultSalesType
                        };
                        // add page button details
                        foreach (var detail in pbuttontoadd.PageButtonDetail)
                        {
                            var pbdetailtoadd = new PageButtonDetail()
                            {
                                AddCost = detail.AddCost,
                                Description = detail.Description,
                                IsRequired = detail.IsRequired,
                                MaxQty = detail.MaxQty,
                                MinQty = detail.MinQty,
                                PriceListDetailId = detail.PriceListDetailId,
                                ProductId = detail.ProductId,
                                Qty = detail.Qty,
                                RemoveCost = detail.RemoveCost,
                                Sort = detail.Sort,
                                Type = detail.Type
                            };

                            pbuttontoadd.PageButtonDetail.Add(pbdetailtoadd);
                        }

                        pagetoadd.PageButton.Add(pbuttontoadd);
                    }

                    pageset.Pages.Add(pagetoadd);
                    // db.Pages.Add(pagetoadd);
                }

                // add copied pagepos assoc
                var pageposassocs = db.PagePosAssoc.Where(f => f.PageSetId == pageSetToCopyId);
                foreach (var pageposassoc in pageposassocs)
                {
                    var assoctoadd = new PagePosAssoc()
                    {
                        // PageSetId = pageset.Id,
                        PosInfoId = pageposassoc.PosInfoId
                    };
                    pageset.PagePosAssoc.Add(assoctoadd);
                }

                // add copied pdamodules
                var pdamodules = db.PdaModule.Where(f => f.PageSetId == pageSetToCopyId);
                foreach (var pdamodule in pdamodules)
                {
                    var pdamoduletoadd = new PdaModule()
                    {
                        //  PageSetId = pageset.Id,
                        MaxActiveUsers = pdamodule.MaxActiveUsers,
                        Description = pdamodule.Description,
                        Code = pdamodule.Code,
                        PosInfoId = pdamodule.PosInfoId,
                        Status = pdamodule.Status
                    };
                    pageset.PdaModule.Add(pdamoduletoadd);

                }


                db.PageSet.Add(pageset);

                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pageset);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pageset.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PageSet/5
        public HttpResponseMessage DeletePageSet(long id)
        {
            PageSet pageset = db.PageSet.Find(id);
            if (pageset == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var PagePosAssocs = db.PagePosAssoc.Where(w => w.PageSetId == id);
            foreach (var p in PagePosAssocs)
            {
                db.PagePosAssoc.Remove(p);
            }

            db.PageSet.Remove(pageset);

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