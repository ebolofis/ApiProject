using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_StaffDT
    {
        /// <summary>
        /// Authenticate Staff 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>StaffId</returns>
        long LoginStaff(DBInfoModel Store, DALoginStaffModel loginStaffModel);

        DA_StaffModel GetStaffById(DBInfoModel Store, long id);

    }
}
