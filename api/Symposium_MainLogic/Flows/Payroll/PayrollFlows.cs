
using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Payroll;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Payroll;
using Symposium.WebApi.MainLogic.Tasks.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Payroll
{

        public class PayrollFlows : IPayrollFlows
        {

            IPayrollTasks payrolltasks;



            public PayrollFlows(IPayrollTasks tasks)
            {
                this.payrolltasks = tasks;
            }
        


        public List<PayrollNewModel> GetAll(DBInfoModel dbInfo)
        {
            return payrolltasks.GetAllPayrole(dbInfo);
        }

        public PayrollNewModel GetTopRowByType(DBInfoModel dbInfo, long StaffId)
        {
            return payrolltasks.GetTopRowByType(dbInfo, StaffId);
        }

        public List<PayrollNewModel> InsertPayroll(DBInfoModel dbinfo, PayrollNewModel model)
        {
            return payrolltasks.InsertPayroll(dbinfo,model);
        }

      public List<PayrollNewModel> Update(DBInfoModel dbinfo, PayrollNewModel model)
        {
            return payrolltasks.Update(dbinfo, model);
        }

        public List<PayrollNewModel> DeletePayroll(DBInfoModel dbinfo, long Id)
        {
            return payrolltasks.DeletePayroll(dbinfo, Id);
        }
    }


    

}
