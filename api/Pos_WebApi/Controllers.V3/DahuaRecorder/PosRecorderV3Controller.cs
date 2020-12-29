
using Symposium.Models.Models.DahuaRecorder;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DahuaRecorder;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DahuaRecorder
{
    [RoutePrefix("api/v3/PosRecorder")]
    public class PosRecorderV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IPosRecorderFlows PosRecorderFlows;

        public PosRecorderV3Controller(IPosRecorderFlows posRecorderFlows)
        {
            this.PosRecorderFlows = posRecorderFlows;
        }

        /// <summary>
        /// Make Connection to TCP Server
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MakeConnection")]
        public HttpResponseMessage MakeConnection()
        {
            bool res = PosRecorderFlows.MakeConnection();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Send Message to TCP Server
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SendMessage")]
        public HttpResponseMessage SendMessage(PosRecorderModel Model)
        {
            bool res = PosRecorderFlows.SendMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}