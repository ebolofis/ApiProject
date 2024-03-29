﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class PosInfoDetail_Excluded_AccountsController : ApiController
    {
        GenericRepository<PosInfoDetail_Excluded_Accounts> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PosInfoDetail_Excluded_AccountsController()
        {
            svc = new GenericRepository<PosInfoDetail_Excluded_Accounts>(db);
        }

        // GET api/AuthorizedGroup
        public IEnumerable<PosInfoDetail_Excluded_Accounts> GetPosInfoDetail_Excluded_Accounts()
        {
            return svc.GetAll();
        }

        public PagedResult<PosInfoDetail_Excluded_Accounts> GetPosInfoDetail_Excluded_Accounts(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        // GET api/AuthorizedGroup/5
        public PosInfoDetail_Excluded_Accounts GetPosInfoDetail_Excluded_Accounts(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        // PUT api/AuthorizedGroup/5
        [HttpPut]
        public HttpResponseMessage PutPosInfoDetail_Excluded_Accounts( PosInfoDetail_Excluded_Accounts model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            svc.Update(model);

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

        // POST api/AuthorizedGroup
        public HttpResponseMessage PostPosInfoDetail_Excluded_Accounts(PosInfoDetail_Excluded_Accounts model)
        {

            if (ModelState.IsValid)
            {

                svc.Add(model);
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

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/AuthorizedGroup/5
        public HttpResponseMessage DeletePosInfoDetail_Excluded_Accounts(long id)
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
