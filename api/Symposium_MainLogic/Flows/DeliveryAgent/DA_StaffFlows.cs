
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_StaffFlows: IDA_StaffFlows
    {
        IDA_StaffTasks staffTasks;
        public DA_StaffFlows(IDA_StaffTasks _staffTasks)
        {
            this.staffTasks = _staffTasks;
        }

        /// <summary>
        /// Authenticate Staff 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>StaffId</returns>
        public long LoginStaff(DBInfoModel dbInfo, DALoginStaffModel loginStaffModel)
        {
            return staffTasks.LoginStaff(dbInfo, loginStaffModel);
        }

        public DA_StaffModel GetStaffById(DBInfoModel dbInfo, long id)
        {
            return staffTasks.GetStaffById(dbInfo, id);
        }
    }
}
