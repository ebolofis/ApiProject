using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class StaffTasks : IStaffTasks 
    {
        IStaffDT staffDT;
        public StaffTasks(IStaffDT staff)
        {
            this.staffDT = staff;
        }

        /// <summary>
        /// Gets all staff from db
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <returns></returns>
        public List<DA_StaffModel> GetAllStaff(DBInfoModel DBInfo)
        {
            List<DA_StaffModel> staff = staffDT.GetAllStaff(DBInfo);
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
            StaffModelsPreview getStaff = staffDT.GetStaffs(Store, storeid, forlogin, posid);

            return getStaff;
        }

        /// <summary>
        /// Return  the DAStore for a specific staff
        /// </summary>
        /// <param name="Store">db connection</param>
        /// <param name="staddId">staff.Id</param>
        /// <returns></returns>
        public long GetDaStore(DBInfoModel Store, long staddId)
        {
            return staffDT.GetDaStore(Store, staddId);
        }

       public long LoyaltyAdminAuthorization(DBInfoModel DBInfo, string loyaltyadminusername, string loyaltyadminpassword )
        {
            return staffDT.LoyaltyAdminAuthorization(DBInfo, loyaltyadminusername, loyaltyadminpassword);
        }

        /// <summary>
        /// check staff credentials supplied from webpos_reports login page to authorize staff to view reports
        /// </summary>
        /// <param name="reportsusername">string</param>
        /// <param name="reportspassword">string</param>
        /// <returns>bool</returns>
        public bool ReportsAuthorization(DBInfoModel DBInfo, string reportsusername, string reportspassword)
        {
            return staffDT.ReportsAuthorization(DBInfo, reportsusername, reportspassword);
        }
    }
}
