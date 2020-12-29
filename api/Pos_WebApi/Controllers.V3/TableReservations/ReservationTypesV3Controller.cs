using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.TableReservations
{
    [RoutePrefix("api/v3/ReservationTypes")]
    public class ReservationTypesV3Controller : BasicV3Controller
    {
        IReservationTypesFlows reservationTypesFlows;

        public ReservationTypesV3Controller(IReservationTypesFlows _reservationTypesFlows)
        {
            this.reservationTypesFlows = _reservationTypesFlows;
        }

        [HttpGet, Route("GetAll")]
        public HttpResponseMessage GetAll()
        {
            List<ReservationTypeModel> result = reservationTypesFlows.GetAllReservationTypes(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
