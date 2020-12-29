using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Payroll
{
    public interface IPayrollDT
    {

        List<PayrollNewModel> GetAllPayrole(DBInfoModel dbInfo);
        PayrollNewModel GetTopRowByType(DBInfoModel dbInfo, long StaffId);
        long InsertPayroll(DBInfoModel dbinfo, PayrollNewModel model);
        int Update(DBInfoModel dbinfo, PayrollNewModel model);
        bool DeletePayroll(DBInfoModel dbinfo, long Id);
    }
}
