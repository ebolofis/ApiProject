﻿using log4net;
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
    public class ProductForBarcodeEodController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET api/Accounts
        public IEnumerable<ProductForBarcodeEod> GetProductForBarcodeEod(string storeid)
        {

            return db.ProductForBarcodeEod.Include("Product.PricelistDetail.Vat").AsNoTracking().AsEnumerable();
        }

        // GET api/Accounts/5
        public ProductForBarcodeEod GetProductForBarcodeEod(long id, string storeid)
        {
            ProductForBarcodeEod model = db.ProductForBarcodeEod.Find(id);
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return model;
        }

        // PUT api/Accounts/5
        public HttpResponseMessage PutProductForBarcodeEod(long id, string storeid, ProductForBarcodeEod model)
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

        // POST api/Accounts
        public HttpResponseMessage PostAccountMapping(ProductForBarcodeEod model, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.ProductForBarcodeEod.Add(model);
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

        // DELETE api/Accounts/5
        public HttpResponseMessage DeleteProductForBarcodeEod(long id, string storeid)
        {
            ProductForBarcodeEod model = db.ProductForBarcodeEod.Find(id);
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.ProductForBarcodeEod.Remove(model);

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
