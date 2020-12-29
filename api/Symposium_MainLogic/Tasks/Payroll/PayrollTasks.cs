using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DT.Payroll;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Payroll
{

    public class PayrollTasks : IPayrollTasks
    {
        IPayrollDT payrolldt;


        public PayrollTasks(IPayrollDT payrollDT)
        {
            this.payrolldt = payrollDT;
        }

        public List<PayrollNewModel> GetAllPayrole(DBInfoModel dbinfo)
        {
            return payrolldt.GetAllPayrole(dbinfo);
        }

        public PayrollNewModel GetTopRowByType(DBInfoModel dbInfo, long StaffId)
        {
            return payrolldt.GetTopRowByType(dbInfo, StaffId);
        }

        public List<PayrollNewModel> InsertPayroll(DBInfoModel dbinfo, PayrollNewModel model)
        {
            long res = 0;
            if (model.DateFrom != null && model.FromPos == false)
            {
                DateTime dstart = DateTime.Parse(model.DateFrom.ToString());
                DateTime dateStart = dstart.ToLocalTime();
                model.DateFrom = dateStart;
            }
            if (model.DateTo != null && model.FromPos == false)
            {
                DateTime dend = DateTime.Parse(model.DateTo.ToString());
                DateTime dateEnd = dend.ToLocalTime();
                model.DateTo = dateEnd;
            }
            if (model.FromPos == true)
            {

            }
            res = payrolldt.InsertPayroll(dbinfo, model);
            return payrolldt.GetAllPayrole(dbinfo);
        }

        public List<PayrollNewModel> Update(DBInfoModel dbinfo, PayrollNewModel model)
        {
            int res = 0;
            if (model.DateFrom != null && model.FromPos == false)
            {
                DateTime dstart = DateTime.Parse(model.DateFrom.ToString());
                DateTime dateStart = dstart.ToLocalTime();
                model.DateFrom = dateStart;
            }
            if (model.DateTo != null && model.FromPos == false)
            {
                DateTime dend = DateTime.Parse(model.DateTo.ToString());
                DateTime dateEnd = dend.ToLocalTime();
                model.DateTo = dateEnd;
            }
            res = payrolldt.Update(dbinfo, model);
            return payrolldt.GetAllPayrole(dbinfo);
        }

        public List<PayrollNewModel> DeletePayroll(DBInfoModel dbinfo, long Id)
        {
            bool res = false;
            res = payrolldt.DeletePayroll(dbinfo, Id);
            return payrolldt.GetAllPayrole(dbinfo);
        }
    }
}

