using log4net;
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class LockersController : ApiController
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        LockerRepository svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LockersController()
        {
            svc = new LockerRepository(db);
        }
        [Route("api/{storeId}/Lockers/{posInfoId}")]
        public async Task<Lockers> GetLockers(string storeId, long posInfoId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new LockerRepository(db);
            return await svc.GetLockers(posInfoId);
        }

        [Route("api/{storeId}/Lockers/{posInfoId}/AddLockers")]
        [HttpPost]
        public async Task<HttpResponseMessage> OpenLockerPost(string storeId, long posInfoId, IEnumerable<LockerDBO> lockers)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new LockerRepository(db);

            foreach (var locker in lockers)
            {
                try
                {
                    var res = await svc.AddLocker(posInfoId, locker);
                    hub.Clients.Group(storeId).newReceipt(storeId + "|" + res.ExtecrName, res.InvoiceId, true, false, PrintType.PrintWhole, null, false);
                    //GetConnections
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.ExpectationFailed };

                }

            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };


        }

        [Route("api/{storeId}/Lockers/{posInfoId}/RemoveLockers")]
        [HttpPost]
        public async Task<HttpResponseMessage> CloseLockerPost(string storeId, IEnumerable<LockerDBO> lockers, bool withDiscount, long posInfoId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new LockerRepository(db);

            foreach (var locker in lockers)
            {
                try
                {
                    var res = await svc.RemoveLocker(posInfoId, locker, withDiscount);
                    foreach (var r in res)
                    {
                        hub.Clients.Group(storeId).newReceipt(storeId + "|" + r.ExtecrName, r.InvoiceId, true, false, PrintType.PrintWhole, null, false);
                    }
                    //GetConnections
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                   // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.ExpectationFailed };

                }
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
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
