using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class FODayFromPMSController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        public string GetFODay(string storeid, Int64 posInfoId)
        {
            db = new PosEntities(false, Guid.Parse(storeid));
            var hotelinfo = db.HotelInfo.FirstOrDefault();

            if (hotelinfo == null || string.IsNullOrEmpty( hotelinfo.HotelUri) || hotelinfo.Type  == (byte)CustomerPolicyEnum.NoCustomers || hotelinfo.Type == (byte)CustomerPolicyEnum.Delivery)
            {
                var pi = db.PosInfo.Where(w => w.Id == posInfoId).FirstOrDefault();
                if (pi != null && pi.FODay != null)
                    return pi.FODay.Value.ToString("yyyy-MM-dd");
                else return DateTime.Now.ToString("yyyy-MM-dd");
            }
            FODay FODay = new FODay();
            var foday = FODay.GetFODay(hotelinfo.HotelUri, hotelinfo.HotelId ?? 0, db);

            return foday;

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