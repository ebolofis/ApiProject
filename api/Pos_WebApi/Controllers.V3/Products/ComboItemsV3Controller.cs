using Symposium.Models.Models;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/ComboItems")]
    public class ComboItemsV3Controller : BasicV3Controller
    {
        IComboFlows comboFlows;

        public ComboItemsV3Controller(IComboFlows comboFlows)
        {
            this.comboFlows = comboFlows;
        }

        /// <summary>
        /// Selects active combo items
        /// </summary>
        /// <returns> List of combo models </returns>
        [HttpGet, Route("AvailableCombos/{DepartmentId}")]
        public HttpResponseMessage AvailableCombos(long DepartmentId)
        {
            List<ComboModel> res = comboFlows.selectActiveComboItems(DBInfo, DepartmentId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
