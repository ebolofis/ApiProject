using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class StaffFlows : IStaffFlows
    {
        IStaffTasks staffTasks;
        public StaffFlows(IStaffTasks staff)
        {
            this.staffTasks = staff;
        }

        /// <summary>
        /// Gets all staff from db
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <returns></returns>
        public List<DA_StaffModel> GetAllStaff(DBInfoModel DBInfo)
        {
            List<DA_StaffModel> staff = staffTasks.GetAllStaff(DBInfo);
            return staff;
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
        public StaffModelsPreview GetStaffs(DBInfoModel Store, string storeid, bool forlogin, long posid)
        {
            // get the results
            StaffModelsPreview getStaff = staffTasks.GetStaffs(Store, storeid, forlogin, posid);

            return getStaff;
        }

        public long LoyaltyAdminAuthorization(DBInfoModel DBInfo, string loyaltyadminusername, string loyaltyadminpassword)
        {
            long StaffId = staffTasks.LoyaltyAdminAuthorization(DBInfo, loyaltyadminusername, loyaltyadminpassword);
            return StaffId;
        }

        /// <summary>
        /// check staff credentials supplied from webpos_reports login page to authorize staff to view reports
        /// </summary>
        /// <param name="reportsusername">string</param>
        /// <param name="reportspassword">string</param>
        /// <returns>bool</returns>
        public bool ReportsAuthorization(DBInfoModel DBInfo, string reportsusername, string reportspassword)
        {
            return staffTasks.ReportsAuthorization(DBInfo, reportsusername, reportspassword);
        }
    }
}
