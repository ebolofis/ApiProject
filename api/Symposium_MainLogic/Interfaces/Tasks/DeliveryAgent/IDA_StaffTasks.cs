using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_StaffTasks
    {
        /// <summary>
        /// Authenticate Staff 
        /// </summary>
        /// <param name="loginStaffModel"></param>
        /// <returns>StaffId</returns>
        long LoginStaff(DBInfoModel dbInfo, DALoginStaffModel loginStaffModel);

        DA_StaffModel GetStaffById(DBInfoModel dbInfo, long id);

    }
}
