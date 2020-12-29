using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Payroll
{
    public interface IPayrollFlows
    {
        // PayrollV3Controller
        List<PayrollNewModel> GetAll(DBInfoModel dbinfo);
        PayrollNewModel GetTopRowByType(DBInfoModel dbInfo, long StaffId);
        List<PayrollNewModel> InsertPayroll(DBInfoModel dbinfo, PayrollNewModel model);
        List<PayrollNewModel> Update(DBInfoModel dbinfo, PayrollNewModel model);
        List<PayrollNewModel> DeletePayroll(DBInfoModel dbinfo, long Id);
    }
}
