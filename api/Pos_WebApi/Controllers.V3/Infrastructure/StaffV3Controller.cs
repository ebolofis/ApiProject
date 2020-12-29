using Microsoft.AspNet.SignalR;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Staff")]
    public class StaffV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IStaffFlows staffFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public StaffV3Controller(IStaffFlows staff)
        {
            this.staffFlows = staff;
        }

        /// <summary>
        /// Gets all staff from db
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllStaff")]
        public HttpResponseMessage GetAllStaff()
        {
            List<DA_StaffModel> results = staffFlows.GetAllStaff(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        /// <summary>
        /// get all active staff for a specific pos. Assigned Positions and Actions per staff are included
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="forlogin">always true</param>
        /// <param name="posid">posinfo.id</param>
        /// <returns>
        /// for every staff return: 
        ///               Id, Code, FirstName, LastName, 
        ///               list of AssignedPositions,  
        ///               IsCheckedIn, ImageUri, 
        ///               list of ActionsId, 
        ///               list of ActionsDescription, 
        ///               password, Identification
        /// </returns>
        [HttpGet, Route("GetStaffs/storeid/{storeid}/forlogin/{forlogin}/posid/{posid}")]
        public HttpResponseMessage GetStaffs(string storeid, bool forlogin, long posid)
        {
            StaffModelsPreview results = staffFlows.GetStaffs(DBInfo, storeid, forlogin, posid);
            return Request.CreateResponse(HttpStatusCode.OK, results.StaffModels); // return results
        }

        [HttpGet, Route("AuthorizationLoyalty/loyaltyadminusername/{loyaltyadminusername}/loyaltyadminpassword/{loyaltyadminpassword}")]
        public HttpResponseMessage LoyaltyAdminAuthorization(string loyaltyadminusername, string loyaltyadminpassword)
        {
            long results = staffFlows.LoyaltyAdminAuthorization(DBInfo, loyaltyadminusername, loyaltyadminpassword);
            return Request.CreateResponse(HttpStatusCode.OK,results); // return results
        }

        /// <summary>
        /// check staff credentials supplied from webpos_reports login page to authorize staff to view reports
        /// </summary>
        /// <param name="reportsusername">string</param>
        /// <param name="reportspassword">string</param>
        /// <returns>bool</returns>
        [HttpPost, Route("ReportsAuthorization")]
        public HttpResponseMessage ReportsAuthorization(StaffLoginModel data)
        {
            if(data == null || data.username == null || data.password == null)
            {
                logger.Error("Web Pos Report Login Failed: No credentials supplied");
                throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGINNOCREDENTIALS);
            }        
            
            bool result = staffFlows.ReportsAuthorization(DBInfo, data.username, data.password);
            return Request.CreateResponse(HttpStatusCode.OK,result.ToString()); 
        }

    }
}