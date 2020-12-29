using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IStaffFlows
    {
        /// <summary>
        /// Gets all staff from db
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <returns></returns>
        List<DA_StaffModel> GetAllStaff(DBInfoModel DBInfo);

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
        StaffModelsPreview GetStaffs(DBInfoModel Store, string storeid, bool forlogin, long posid);

        long LoyaltyAdminAuthorization(DBInfoModel DBInfo,string loyaltyadminusername,string loyaltyadminpassword);

        /// <summary>
        /// check staff credentials supplied from webpos_reports login page to authorize staff to view reports
        /// </summary>
        /// <param name="reportsusername">string</param>
        /// <param name="reportspassword">string</param>
        /// <returns>bool</returns>
        bool ReportsAuthorization(DBInfoModel DBInfo,string reportsusername, string reportspassword);
    }
}
