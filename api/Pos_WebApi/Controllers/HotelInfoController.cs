using log4net;
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using Symposium.Helpers.Classes;
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
    [Authorize]
    public class HotelInfoController : ApiController
    {
        private PosEntities db = new PosEntities(false);

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET api/HotelInfo
        /// <summary>
        /// Default Get All hotel info entities from databse enstablishing connection over protel Pms interface
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>Enstablished Connection with Pms and returns dynamic Array of hotelinfo entities</returns>
        public IEnumerable<HotelInfo> GetHotelInfo(string storeid)
        {
            //create stored procedure GetReservationInfo2
            foreach (var item in db.HotelInfo)
            {
                pmsConnInit(item);
            }
            return db.HotelInfo.AsEnumerable();
        }

        /// <summary>
        /// Resource to get only db entities without enstablishing connection to hotel info
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        [Route("api/{storeid}/HotelInfo/HotelInfoEntities")]
        public IEnumerable<HotelInfo> GetHotelInfoEntities(string storeid) {
            db = new PosEntities(false, Guid.Parse(storeid));
            return db.HotelInfo.AsEnumerable();
        }

        public Object GetStores(string storeid, bool forhotelinfo)
        {
            var h = db.HotelInfo.FirstOrDefault();
            return h != null ? h : null;
        }

        // GET api/HotelInfo/5
        public HotelInfo GetHotelInfo(long id, string storeid)
        {
            HotelInfo HotelInfo = db.HotelInfo.Find(id);
            if (HotelInfo == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return HotelInfo;
        }

        // PUT api/HotelInfo/5
        public HttpResponseMessage PutHotelInfo(long id, string storeid, HotelInfo HotelInfo)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != HotelInfo.Id)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (!string.IsNullOrEmpty(HotelInfo.DBPassword) && HotelInfo.DBPassword.Length < 20)
                HotelInfo.DBPassword = StringCipher.Encrypt(HotelInfo.DBPassword);
            db.Entry(HotelInfo).State = EntityState.Modified;

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



        // POST api/HotelInfo
        public HttpResponseMessage PostHotelInfo(HotelInfo HotelInfo, string storeid)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(HotelInfo.DBPassword))
                    HotelInfo.DBPassword = StringCipher.Encrypt(HotelInfo.DBPassword);
                db.HotelInfo.Add(HotelInfo);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, HotelInfo);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = HotelInfo.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/HotelInfo/5
        public HttpResponseMessage DeleteHotelInfo(long id, string storeid)
        {
            HotelInfo HotelInfo = db.HotelInfo.Find(id);
            if (HotelInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.HotelInfo.Remove(HotelInfo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, HotelInfo);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }


        public bool pmsConnInit(HotelInfo hotelInfo)
        {
            try
            {

                string connStr = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName ?? "proteluser" +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) ?? "protel915930" + ";database=" + hotelInfo.DBName ?? "protel" + ";";

                PMSConnection pmsConn = new PMSConnection();
                if (pmsConn.initConn(connStr) == true)
                {
                    bool res = pmsConn.makeProcedure(hotelInfo.DBUserName, hotelInfo.HotelType);
                    pmsConn.closeConnection();
                    return res;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}